using System.Security.Claims;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IBidItemService
{
    Task<BidItemResponseDto> PlaceBidAsync(BidItemAddDto bidDto);
    Task<IEnumerable<BidItemResponseDto>> GetAllBidAsync(BidFilterDto filter);
    Task<IEnumerable<BidItemResponseDto>> GetBidsByAuctionIdAsync(Guid auctionId, ClaimsPrincipal user);
    Task<BidItemResponseDto?> GetHighestBidAsync(Guid auctionItemId);
    Task<bool> DeleteBidAsync(Guid bidId);
    Task<bool> UpdateBidStatusAsync(Guid bidId, BidStatus newStatus);
    Task<IEnumerable<BidItemResponseDto>> GetBidsByUserIdAsync(Guid userId, BidFilterDto filter);
    Task<BidItemResponseDto> GetBidsByBiddingIdAsync(Guid biddingId);
}
