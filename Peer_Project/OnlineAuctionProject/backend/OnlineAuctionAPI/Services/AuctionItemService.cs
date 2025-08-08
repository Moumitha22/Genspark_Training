using AutoMapper;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Hubs;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Repositories;

public class AuctionItemService : IAuctionItemService
{
    private readonly IAuctionItemRepository _auctionRespository;
    private readonly IUserRepository _userRepository;
    private readonly IBidItemRepository _bidItemRepository;
    private readonly IVirtualWalletRepository _virtualWalletRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuctionContext _auctionContext;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly IEAgreementRepository _eAgreementRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<AuctionHub> _hubContext;

    public AuctionItemService(
        IAuctionItemRepository auctionRepository,
        IUserRepository userRepository,
        IBidItemRepository bidItemRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        AuctionContext auctionContext,
        IUserService userService,
        IEmailService emailService,
        IVirtualWalletRepository virtualWalletRepository,
        IEAgreementRepository eAgreementRepository,
        IHubContext<AuctionHub> hubContext
    )
    {
        _auctionRespository = auctionRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _bidItemRepository = bidItemRepository;
        _auctionContext = auctionContext;
        _userService = userService;
        _emailService = emailService;
        _virtualWalletRepository = virtualWalletRepository;
        _eAgreementRepository = eAgreementRepository;
        _hubContext = hubContext;
    }

    public async Task<AuctionItemResponseDto> AddAuctionItemAsync(AuctionItemAddDto auctionDto)
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c =>
            c.Type == "UserId"
        );
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var sellerId))
        {
            throw new InvalidDataException("You are not authorized to add an auction.");
        }
        if (auctionDto.StartTime < DateTime.UtcNow)
        {
            throw new InvalidException("Auction start time cannot be in the past.");
        }
        if (auctionDto.EndTime <= auctionDto.StartTime)
        {
            throw new InvalidException("Auction end time must be after the start time.");
        }
        if (auctionDto.StartingPrice <= 0)
        {
            throw new InvalidException("Starting price must be greater than zero.");
        }
        if (auctionDto.ReservePrice < auctionDto.StartingPrice)
        {
            throw new InvalidException("Reserve price cannot be less than starting price.");
        }
        if (string.IsNullOrWhiteSpace(auctionDto.Name))
            throw new ArgumentException(
                $"{nameof(auctionDto.Name)} must not be null, empty or whitespace"
            );

        auctionDto.SellerId = sellerId;
        var seller = await _userRepository.Get(auctionDto.SellerId);

        if (seller == null)
        {
            throw new NotFoundException($"Seller Id with {auctionDto.SellerId} is not found.");
        }

        var auctionItem = _mapper.Map<AuctionItem>(auctionDto);
        auctionItem.CreatedAt = DateTime.UtcNow;
        auctionItem.UpdatedAt = DateTime.UtcNow;

        if (auctionItem.Status == AuctionStatus.Live && auctionItem.StartTime > DateTime.UtcNow)
        {
            throw new InvalidException(
                "Auction cannot be set to Live if the start time is in the future."
            );
        }
        if (
            auctionItem.Status == AuctionStatus.Upcoming
            && auctionItem.StartTime <= DateTime.UtcNow
        )
        {
            throw new InvalidException(
                "Auction cannot be set to Upcoming if the start time is now or in the past."
            );
        }

        if (auctionItem.FileAttachments == null)
        {
            auctionItem.FileAttachments = new List<FileData>();
        }

        if (auctionDto.FileAttachments != null && auctionDto.FileAttachments.Any())
        {
            foreach (var fileattachment in auctionDto.FileAttachments)
            {
                using var memoryStream = new MemoryStream();
                await fileattachment.CopyToAsync(memoryStream);
                var fileData = new FileData
                {
                    Name = fileattachment.FileName,
                    ContentType = fileattachment.ContentType,
                    Data = memoryStream.ToArray(),
                };
                auctionItem.FileAttachments.Add(fileData);
            }
        }

        await _auctionRespository.Add(auctionItem);

        var notification = new Notification
        {
            Message = $"New auction created: {auctionItem.Name}",
            CreatedAt = DateTime.UtcNow,
        };

        _auctionContext.Notifications.Add(notification);
        await _auctionContext.SaveChangesAsync();

        var responseDto = _mapper.Map<AuctionItemResponseDto>(
            auctionItem,
            opts =>
            {
                opts.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );

        await _hubContext.Clients.Group("Bidders").SendAsync("AuctionItemAdded", responseDto);
        return responseDto;
    }

    public async Task<bool> DeleteAuctionItemAsync(Guid id)
    {
        var auctionItem = await _auctionRespository.GetAuctionsWithFile(id);
        if (auctionItem == null)
        {
            throw new NotFoundException(
                "There is no data present in the db with this particular id. Please verify it"
            );
        }

        var bids = await _bidItemRepository.GetBidsByAuctionAsync(id);
        if (bids != null)
        {
            foreach (var bid in bids)
            {
                await _bidItemRepository.Delete(bid.Id);
            }
        }

        await _auctionRespository.Delete(id);
        return true;
    }

    public async Task<IEnumerable<AuctionItemResponseDto>> GetAllAuctionItemAsync()
    {
        var auctionItems = await _auctionRespository.GetAllAuctionsWithFile();
        if (auctionItems == null || !auctionItems.Any())
        {
            throw new NotFoundException(
                "There is no data present in the auctions, please try after a while"
            );
        }
        return _mapper.Map<IEnumerable<AuctionItemResponseDto>>(
            auctionItems,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );
    }

    public async Task<IEnumerable<AuctionItemResponseDto>> GetAllAuctionItemBySellerAsync(
        Guid sellerId
    )
    {
        var auctionItems = await _auctionRespository.GetAllAuctionsWithFile();

        var sellerAuctions = auctionItems?.Where(a => a.SellerId == sellerId).ToList();

        if (sellerAuctions == null || !sellerAuctions.Any())
        {
            throw new NotFoundException("No auctions found for this seller.");
        }

        return _mapper.Map<IEnumerable<AuctionItemResponseDto>>(
            sellerAuctions,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );
    }

    public async Task<AuctionItemResponseDto?> GetAuctionItemByIdAsync(Guid id)
    {
        var auctionItem = await _auctionRespository.GetAuctionsWithFile(id);
        if (auctionItem == null)
        {
            throw new NotFoundException($"Auction item with ID {id} is not found");
        }
        return _mapper.Map<AuctionItemResponseDto>(
            auctionItem,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );
    }

    public async Task<AuctionItemResponseDto?> UpdateAuctionItemAsync(
        Guid id,
        AuctionItemAddDto auctionUpdateDto
    )
    {
        var auctionItem = await _auctionRespository.GetAuctionsWithFile(id);

        if (auctionItem == null)
            throw new NotFoundException($"Auction item with ID {id} not found");

        if (!string.IsNullOrWhiteSpace(auctionUpdateDto.Name))
            auctionItem.Name = auctionUpdateDto.Name;

        if (!string.IsNullOrWhiteSpace(auctionUpdateDto.Description))
            auctionItem.Description = auctionUpdateDto.Description;

        if (auctionUpdateDto.StartingPrice != default(decimal))
            auctionItem.StartingPrice = auctionUpdateDto.StartingPrice;

        if (auctionUpdateDto.ReservePrice != default(decimal))
            auctionItem.ReservePrice = auctionUpdateDto.ReservePrice;

        if (auctionUpdateDto.StartTime != default(DateTime))
            auctionItem.StartTime = DateTime.SpecifyKind(auctionUpdateDto.StartTime,DateTimeKind.Utc);
 
        if (auctionUpdateDto.EndTime != default(DateTime))
            auctionItem.EndTime = DateTime.SpecifyKind(auctionUpdateDto.EndTime, DateTimeKind.Utc);

        if (!string.IsNullOrWhiteSpace(auctionUpdateDto.Status))
        {
            if (Enum.TryParse<AuctionStatus>(auctionUpdateDto.Status, out var statusEnum))
            {
                auctionItem.Status = statusEnum;
            }
            else
            {
                throw new ArgumentException($"Invalid status value: {auctionUpdateDto.Status}");
            }
        }

        if (auctionUpdateDto.SellerId != Guid.Empty)
        {
            var validSeller = await _userRepository.Get(auctionUpdateDto.SellerId);
            if (validSeller != null && validSeller.Role == UserRole.Seller)
                auctionItem.SellerId = auctionUpdateDto.SellerId;
            else
                throw new InvalidException("Invalid Seller Id");
        }

        if (auctionUpdateDto.FileAttachments != null && auctionUpdateDto.FileAttachments.Any())
        {
            auctionItem?.FileAttachments?.Clear();
            foreach (var fileattachment in auctionUpdateDto.FileAttachments)
            {
                using var memoryStream = new MemoryStream();
                await fileattachment.CopyToAsync(memoryStream);

                auctionItem?.FileAttachments?.Add(
                    new FileData
                    {
                        Name = fileattachment.FileName,
                        ContentType = fileattachment.ContentType,
                        Data = memoryStream.ToArray(),
                    }
                );
            }
        }

        auctionItem!.UpdatedAt = DateTime.UtcNow;

        await _auctionRespository.Update(id, auctionItem);

        return _mapper.Map<AuctionItemResponseDto>(
            auctionItem,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );
    }

    public async Task<PaginatedResponseDto<AuctionItemResponseDto>> GetPagedAuctionItemsAsync(
        PaginationDto pagination
    )
    {
        var query = _auctionRespository.GetAllQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Name))
        {
            var nameLower = pagination.Name.Trim().ToLower();
            query = query.Where(a => a.Name.ToLower().Contains(nameLower));
        }

        if (pagination.StartingPriceMin.HasValue)
        {
            query = query.Where(a => a.StartingPrice >= pagination.StartingPriceMin.Value);
        }
        if (pagination.StartingPriceMax.HasValue)
        {
            query = query.Where(a => a.StartingPrice <= pagination.StartingPriceMax.Value);
        }

        if (pagination.ReservePriceMin.HasValue)
        {
            query = query.Where(a => a.ReservePrice >= pagination.ReservePriceMin.Value);
        }
        if (pagination.ReservePriceMax.HasValue)
        {
            query = query.Where(a => a.ReservePrice <= pagination.ReservePriceMax.Value);
        }

        if (pagination.StartTime.HasValue)
        {

            var startUtc = ToUtcFromIst(pagination.StartTime.Value);
            query = query.Where(a => a.StartTime >= startUtc);
        }

        if (pagination.EndTime.HasValue)
        {
            var endUtc = ToUtcFromIst(pagination.EndTime.Value); 
            query = query.Where(a => a.EndTime <= endUtc);
        }

        if (!string.IsNullOrWhiteSpace(pagination.Status))
        {
            if (Enum.TryParse<AuctionStatus>(pagination.Status, true, out var statusEnum))
            {
                query = query.Where(a => a.Status == statusEnum);
            }
        }

        if (!string.IsNullOrWhiteSpace(pagination.SellerId))
        {
            query = query.Where(a => a.SellerId == Guid.Parse(pagination.SellerId));
        }
        
        if (pagination.HasFileAttachments.HasValue)
        {
            if (pagination.HasFileAttachments.Value)
            {
                query = query.Where(a => a.FileAttachments != null && a.FileAttachments.Any());
            }
            else
            {
                query = query.Where(a => a.FileAttachments == null || !a.FileAttachments.Any());
            }
        }

        if (!string.IsNullOrWhiteSpace(pagination.FileName))
        {
            var fileNameLower = pagination.FileName.Trim().ToLower();
            query = query.Where(a => a.FileAttachments.Any(f => f.Name.ToLower().Contains(fileNameLower)));
        }

        if (!string.IsNullOrWhiteSpace(pagination.SortBy))
        {
            bool ascending = pagination.SortDirection?.ToLower() != "desc";
            switch (pagination.SortBy.ToLower())
            {
                case "name":
                    query = ascending
                        ? query.OrderBy(a => a.Name).ThenBy(a => a.Id)
                        : query.OrderByDescending(a => a.Name).ThenByDescending(a => a.Id);
                    break;
                case "starttime":
                    query = ascending
                        ? query.OrderBy(a => a.StartTime).ThenBy(a => a.Id)
                        : query.OrderByDescending(a => a.StartTime).ThenByDescending(a => a.Id);
                    break;
                case "endtime":
                    query = ascending
                        ? query.OrderBy(a => a.EndTime).ThenBy(a => a.Id)
                        : query.OrderByDescending(a => a.EndTime).ThenByDescending(a => a.Id);
                    break;
                case "createdat":
                    query = ascending
                        ? query.OrderBy(a => a.CreatedAt).ThenBy(a => a.Id)
                        : query.OrderByDescending(a => a.CreatedAt).ThenByDescending(a => a.Id);
                    break;
                case "deleterequest":
                    query = ascending
                        ? query.OrderBy(a => a.DeleteRequest == null)
                                .ThenBy(a => a.DeleteRequest != null ? a.DeleteRequest.RequestDate : DateTime.MaxValue)
                                .ThenBy(a => a.Id)
                        : query.OrderByDescending(a => a.DeleteRequest != null)
                                .ThenByDescending(a => a.DeleteRequest != null ? a.DeleteRequest.RequestDate : DateTime.MinValue)
                                .ThenByDescending(a => a.Id);
                    break;
                default:
                    query = ascending
                        ? query.OrderBy(a => a.Name).ThenBy(a => a.Id)
                        : query.OrderByDescending(a => a.Name).ThenByDescending(a => a.Id);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(a => a.Name).ThenBy(a => a.Id);
        }

        int totalRecords = await query.CountAsync();
        int totalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize);

        var pagedItems = await query
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        var resultDto = _mapper.Map<IEnumerable<AuctionItemResponseDto>>(
            pagedItems,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );

        return new PaginatedResponseDto<AuctionItemResponseDto>
        {
            Data = resultDto,
            Pagination = new PaginationMetaData
            {
                TotalRecords = totalRecords,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalPages = totalPages,
            },
        };
    }

    public static DateTime ToUtcFromIst(DateTime istTime)
    {
        var istZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        var localTime = DateTime.SpecifyKind(istTime, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(localTime, istZone);
    }

    public async Task<WinnerIdResponseDto> UpdateWinningId(WinningIdUpdateDto dto)
    {
        if (dto == null)
            throw new ArgumentException("Invalid winning ID update data");
        var bid = await _bidItemRepository.GetByIdAsync(dto.WinningId);

        if (bid == null)
            throw new NotFoundException("Bid not found for the provided WinningId.");

        if (bid.AuctionItemId != dto.AuctionItemId)
            throw new InvalidException("Bid does not belong to the specified auction item.");

        var auctionItem = await _auctionRespository.GetAuctionsWithFile(dto.AuctionItemId);

        if (auctionItem == null)
            throw new NotFoundException($"Auction item with ID {dto.AuctionItemId} not found");

        var maxAmount = await _bidItemRepository.GetMaxBidAmountAsync(
            dto.AuctionItemId,
            auctionItem.EndTime
        );

        if (maxAmount == null)
            throw new InvalidException("No valid bids found for this auction item.");

        var highestBids = await _bidItemRepository.GetHighestBidsAsync(
            dto.AuctionItemId,
            auctionItem.EndTime,
            maxAmount.Value
        );

        var isWinningBid = highestBids.Any(b => b.Id == bid.Id);

        if (!isWinningBid)
            throw new InvalidException(
                "Only a bid with the highest amount can be declared as the winner."
            );

        var winningBid = highestBids.First();

        if (bid.Id != winningBid.Id)
            throw new InvalidException(
                "Only the earliest bid with the highest amount can be declared as the winner."
            );

        var winnerWallet = await _virtualWalletRepository.GetByUserIdAsync(bid.BidderId);

        if (winnerWallet == null)
            throw new NotFoundException("Winner's virtual wallet not found.");

        if (winnerWallet.Balance < bid.Amount)
            throw new InvalidException("Insufficient balance in winner's virtual wallet.");

        winnerWallet.Balance -= bid.Amount;
        winnerWallet.UpdatedAt = DateTime.UtcNow;
        await _virtualWalletRepository.Update(winnerWallet.Id, winnerWallet);

        var history = new VirtualWalletHistory
        {
            VirtualWalletId = winnerWallet.Id,
            Amount = -bid.Amount,
            Description = $"Deducted for winning auction {auctionItem.Id}",
            TransactionDate = DateTime.UtcNow,
        };

        await _virtualWalletRepository.AddHistoryAsync(history);

        auctionItem.WinnerId = bid.Id;
        auctionItem.Status = AuctionStatus.Completed;
        await _auctionRespository.Update(auctionItem.Id, auctionItem);

        byte[] pdfBytes = PdfHelper.GenerateAgreementPdf(auctionItem, bid);

        var agreement = new EAgreement
        {
            Id = Guid.NewGuid(),
            AuctionItemId = auctionItem.Id,
            BiddingId = bid.Id,
            File = pdfBytes,
            CreatedAt = DateTime.UtcNow,
        };
        await _eAgreementRepository.Add(agreement);

        var recipient =
            await _userService.GetUserByIdAsync(bid.BidderId)
            ?? throw new NotFoundException($"Recipient with ID {bid.BidderId} not found.");
        var pdfFilePath = PdfHelper.SavePdfToFile(
            pdfBytes,
            $"agreement_{agreement.Id} - {recipient.Username} .pdf"
        );

        await _emailService.SendEmailAsync(
            recipient.Email,
            "Auction Won",
            $"Congratulations! You have won the auction for {auctionItem.Name}.",
            pdfFilePath
        );

        var winner = await _userRepository.Get(bid.BidderId);

        return new WinnerIdResponseDto
        {
            AuctionItemId = dto.AuctionItemId,
            WinnerId = winner?.Id ?? Guid.Empty,
            WinnerName = winner?.Username ?? "Unknown",
            WinningPrice = bid.Amount,
        };
    }

    public async Task<AuctionItemResponseDto> UpdateAuctionStatusAsync(
        Guid auctionItemId,
        AuctionStatus newStatus
    )
    {
        var auctionItem = await _auctionRespository.GetAuctionsWithFile(auctionItemId);
        if (auctionItem == null)
            throw new NotFoundException($"Auction item with ID {auctionItemId} not found");

        if (!Enum.IsDefined(typeof(AuctionStatus), newStatus))
            throw new ArgumentException($"Invalid status value: {newStatus}");

        if (
            newStatus != AuctionStatus.Completed
            && newStatus != AuctionStatus.Cancelled
            && newStatus != AuctionStatus.Live
        )
        {
            throw new InvalidException("Cannot change status of a completed or cancelled auction.");
        }

        if (
            (auctionItem.Status == AuctionStatus.Completed && newStatus != AuctionStatus.Completed)
            || (
                auctionItem.Status == AuctionStatus.Cancelled
                && newStatus != AuctionStatus.Cancelled
            )
        )
            throw new InvalidException(
                "Cannot change status of a completed auction or cancelled auction."
            );

        auctionItem.Status = newStatus;
        auctionItem.UpdatedAt = DateTime.UtcNow;
        await _auctionRespository.Update(auctionItemId, auctionItem);

        var result = _mapper.Map<AuctionItemResponseDto>(
            auctionItem,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );
        await _hubContext.Clients.Group("Bidders").SendAsync("AuctionStatusUpdated", result);
        return result;
    }

    public async Task<IEnumerable<AuctionItemResponseDto>> GetActiveAuctionsAsync()
    {
        var activeAuctions = await _auctionRespository
            .GetAllQueryable()
            .Where(a => a.Status == AuctionStatus.Live)
            .ToListAsync();

        return _mapper.Map<IEnumerable<AuctionItemResponseDto>>(
            activeAuctions,
            opt =>
            {
                opt.Items["HttpContext"] = _httpContextAccessor.HttpContext!;
            }
        );
    }

    public async Task<WinnerIdResponseDto> GetAuctionWinnerAsync(Guid auctionItemId)
    {
        var auctionItem = await _auctionRespository.GetAuctionsWithFile(auctionItemId);
        if (auctionItem == null)
            throw new NotFoundException($"Auction item with ID {auctionItemId} not found");

        if (auctionItem.WinnerId == null)
            throw new NullValueException("This auction has not declared a winner yet.");

        var bid = await _bidItemRepository.GetByIdAsync(auctionItem.WinnerId.Value);
        if (bid == null)
            throw new NotFoundException("No winning bid found for this auction item.");

        var user = await _userRepository.Get(bid.BidderId);
        if (user == null)
            throw new NotFoundException("No user found for the winning bid.");

        return new WinnerIdResponseDto
        {
            WinnerId = user.Id,
            WinnerName = user.Username,
            WinningPrice = bid.Amount,
            AuctionItemId = auctionItemId,
        };
    }

    public async Task<decimal> GetTotalSales(Guid sellerId)
    {
        if (sellerId == Guid.Empty)
        {
            throw new ArgumentException("Seller ID cannot be empty.");
        }
        var sellerAuctions = await GetAllAuctionItemBySellerAsync(sellerId);
        if (sellerAuctions == null || !sellerAuctions.Any())
        {
            throw new NotFoundException("No auctions found for this seller.");
        }

        var completedAuctions = sellerAuctions
            .Where(a => a.Status == AuctionStatus.Completed.ToString() && a.WinnerId != null)
            .ToList();

        decimal totalSales = 0m;
        foreach (var auction in completedAuctions)
        {
            if (Guid.TryParse(auction.WinnerId, out var winningBidId))
            {
                var bid = await _bidItemRepository.GetByIdAsync(winningBidId);
                if (bid != null)
                {
                    totalSales += bid.Amount;
                }
            }
            else
            {
                throw new InvalidException(
                    $"Invalid WinnerId format for auction item {auction.Id}."
                );
            }
        }
        return totalSales;
    }
}
