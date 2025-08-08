using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;
using OnlineAuctionAPI.Services;

namespace OnlineAuctionAPI.Service;

public class BidItemService : IBidItemService
{
    private readonly IBidItemRepository _bidItemRepository;
    private readonly IAuctionItemRepository _auctionItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHubContext<AuctionHub> _hubContext;

    public BidItemService(
        IBidItemRepository bidItemRepository,
        IAuctionItemRepository auctionItemRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IHubContext<AuctionHub> hubContext
    )
    {
        _bidItemRepository = bidItemRepository;
        _auctionItemRepository = auctionItemRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _hubContext = hubContext;
    }

    public async Task<BidItemResponseDto> PlaceBidAsync(BidItemAddDto bidDto)
    {
        var auctionItem = await _auctionItemRepository.Get(bidDto.AuctionItemId);
        if (auctionItem == null)
        {
            throw new NullValueException("Auction not found, please enter valid one.");
        }
        if (auctionItem.Status != AuctionStatus.Live)
        {
            throw new InvalidDataException("Auction is not live.");
        }
        // if (auctionItem.EndTime < DateTime.UtcNow)
        // {
        //     throw new InvalidDataException("Auction has ended.");
        // }
        if (auctionItem.StartTime > DateTime.UtcNow)
        {
            throw new InvalidDataException("Auction has not started yet.");
        }
        if (auctionItem.WinnerId != null)
        {
            throw new InvalidDataException("This auction has decalred the winner.");
        }
        if (auctionItem.StartingPrice > bidDto.Amount)
        {
            throw new InvalidDataException(
                "Please place the bid above the starting price of the auctions."
            );
        }
        if (bidDto.Amount <= 0)
        {
            throw new InvalidDataException("Bid amount must be greater than zero.");
        }

        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
            c.Type == "UserId"
        );
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var loggedInUserId))
        {
            throw new InvalidDataException("You are not authorized to place this bid.");
        }

        bidDto.BidderId = loggedInUserId;
        var user = await _userRepository.GetByIdWithVirtualWalletAsync(loggedInUserId);
        if (user?.VirtualWallet == null)
            throw new InvalidDataException(
                "You do not have a virtual wallet. Please create one before bidding."
            );
        if (user.VirtualWallet.Balance < bidDto.Amount)
            throw new InvalidDataException("Insufficient wallet balance to place this bid.");

        var highestBid = await _bidItemRepository.GetHighestBidAsync(bidDto.AuctionItemId);

        if (highestBid != null)
        {
            if (bidDto.Amount <= highestBid.Amount)
                throw new InvalidDataException(
                    $"Your bid must be higher than the current highest bid ({highestBid.Amount}) - for the auction {bidDto.AuctionItemId}."
                );
        }
        else
        {
            if (bidDto.Amount < auctionItem.StartingPrice)
                throw new InvalidDataException(
                    "Please place the bid above the starting price of the auction."
                );
        }
        var existingBids = await _bidItemRepository.GetBidsByAuctionAsync(bidDto.AuctionItemId);
        var userPreviousBid = existingBids?.FirstOrDefault(b => b.BidderId == bidDto.BidderId);

        if (userPreviousBid != null && bidDto.Amount <= userPreviousBid.Amount)
        {
            throw new InvalidDataException(
                "Your new bid must be greater than your previous bid for this auction."
            );
        }

        var bidItem = _mapper.Map<BidItem>(bidDto);
        await _bidItemRepository.Add(bidItem);
        var response = _mapper.Map<BidItemResponseDto>(bidItem);
        await _hubContext.Clients.Group("Bidders").SendAsync("BidPlaced", response);

        return response;
    }

    public async Task<IEnumerable<BidItemResponseDto>> GetAllBidAsync(BidFilterDto filter)
    {
        var allBidsQuery = _bidItemRepository.GetAllQueryable();
        var filteredQuery = ApplyBidFilters(allBidsQuery, filter);
        var bids = await filteredQuery.ToListAsync();

        if (!bids.Any())
        {
            throw new RepositoryOperationException(
            "No bids available right now",
            new NotFoundException("No bids available right now")
        );
        }
        return _mapper.Map<IEnumerable<BidItemResponseDto>>(bids);
    }

    public async Task<IEnumerable<BidItemResponseDto>> GetBidsByAuctionIdAsync(Guid auctionId, ClaimsPrincipal user)
    {
        var role = user.FindFirst(ClaimTypes.Role)?.Value;
        bool includeArchived = role == "Admin";
        var bids = await _bidItemRepository.GetBidsByAuctionAsync(auctionId, includeArchived);

        if (bids == null)
        {
            throw new NotFoundException($"No bids available for this auction{auctionId}");
        }

        return _mapper.Map<IEnumerable<BidItemResponseDto>>(bids);
    }

    public async Task<IEnumerable<BidItemResponseDto>> GetBidsByUserIdAsync(Guid userId, BidFilterDto filter)
    {
        if (userId == Guid.Empty)
        {
            throw new NullValueException("User ID cannot be empty");
        }

        var queryable = _bidItemRepository.GetAllQueryable();

        var filteredQuery = ApplyBidFilters(queryable, filter, userId);

        var bids = await filteredQuery.ToListAsync();

        if (bids == null)
        {
            throw new NotFoundException($"No bids available for user {userId}");
        }

        return _mapper.Map<IEnumerable<BidItemResponseDto>>(bids);
    }

    public async Task<BidItemResponseDto?> GetHighestBidAsync(Guid auctionItemId)
    {
        var highestBid = await _bidItemRepository.GetHighestBidAsync(auctionItemId);
        return _mapper.Map<BidItemResponseDto>(highestBid);
    }

    public async Task<bool> DeleteBidAsync(Guid bidId)
    {
        var bid = await _bidItemRepository.Get(bidId);
        if (bid == null)
        {
            throw new NotFoundException($"Bid with ID {bidId} not found");
        }
        var deleteBidItem = await _bidItemRepository.Delete(bid.Id);
        if (deleteBidItem == null)
        {
            throw new NotFoundException($"Bid with ID {bidId} not found");
        }
        return true;
    }

    public async Task<bool> UpdateBidStatusAsync(Guid bidId, BidStatus newStatus)
    {
        var bid = await _bidItemRepository.Get(bidId);
        if (bid == null)
        {
            throw new NotFoundException($"Bid with ID {bidId} not found");
        }

        bid.Status = newStatus;
        var updatedBidItem = await _bidItemRepository.Update(bid.Id, bid);
        if (updatedBidItem == null)
        {
            throw new NotFoundException($"Bid with ID {bidId} not found");
        }
        return true;
    }

    public async Task<BidItemResponseDto> GetBidsByBiddingIdAsync(Guid biddingId)
    {
        if (biddingId == Guid.Empty)
        {
            throw new NullValueException("Bidding ID cannot be empty");
        }

        var bids = await _bidItemRepository.GetByIdAsync(biddingId);
        if (bids == null)
        {
            throw new NotFoundException($"No bids available for bidding {biddingId}");
        }

        return _mapper.Map<BidItemResponseDto>(bids);
    }

    private IQueryable<BidItem> ApplyBidFilters(IQueryable<BidItem> query, BidFilterDto filter, Guid? userId = null)
    {
        if (userId.HasValue && userId.Value != Guid.Empty)
        {
            query = query.Where(b => b.BidderId == userId.Value && b.Status == BidStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var lowerName = filter.Name.ToLower();
            query = query.Where(b => b.AuctionItem.Name.ToLower().Contains(lowerName));
        }

        if (filter.AmountMin.HasValue)
        {
            query = query.Where(b => b.Amount >= filter.AmountMin.Value);
        }

        if (filter.AmountMax.HasValue)
        {
            query = query.Where(b => b.Amount <= filter.AmountMax.Value);
        }

        if (filter.DateMin.HasValue)
        {
            var startUtc = ToUtcFromIst(filter.DateMin.Value);
            query = query.Where(b => b.BidTime >= startUtc);
        }

        if (filter.DateMax.HasValue)
        {
            var endUtc = ToUtcFromIst(filter.DateMax.Value);
            query = query.Where(b => b.BidTime <= endUtc);
        }

        if (!string.IsNullOrWhiteSpace(filter.SortBy))
        {
            bool ascending = filter.SortDirection?.ToLower() != "desc";
            switch (filter.SortBy.ToLower())
            {
                case "amount":
                    query = ascending
                        ? query.OrderBy(b => b.Amount).ThenBy(b => b.Id)
                        : query.OrderByDescending(b => b.Amount).ThenByDescending(b => b.Id);
                    break;
                case "date":
                    query = ascending
                        ? query.OrderBy(b => b.BidTime).ThenBy(b => b.Id)
                        : query.OrderByDescending(b => b.BidTime).ThenByDescending(b => b.Id);
                    break;
                case "name":
                    query = ascending
                        ? query.OrderBy(b => b.AuctionItem.Name).ThenBy(b => b.Id)
                        : query.OrderByDescending(b => b.AuctionItem.Name).ThenByDescending(b => b.Id);
                    break;
                default:
                    query = query.OrderBy(b => b.BidTime).ThenBy(b => b.Id);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(b => b.BidTime).ThenBy(b => b.Id);
        }

        return query;
    }
    
    public static DateTime ToUtcFromIst(DateTime istTime)
    {
        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        var localTime = DateTime.SpecifyKind(istTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localTime, istZone);
    }
}
