using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Interfaces;

public interface IBidItemRepository : IRepository<Guid, BidItem>
{
    Task<IEnumerable<BidItem>> GetBidsByAuctionAsync(Guid auctionItemId, bool includeArchived = false);
    Task<BidItem?> GetHighestBidAsync(Guid auctionItemId);

    Task<IEnumerable<BidItem>> GetBidsByUserIdAsync(Guid userId);
    Task<BidItem?> GetByIdAsync(Guid bidId);
    Task<decimal?> GetMaxBidAmountAsync(Guid auctionItemId, DateTime endTime);
    Task<List<BidItem>> GetHighestBidsAsync(
        Guid auctionItemId,
        DateTime endTime,
        decimal maxAmount
    );
    public IQueryable<BidItem> GetAllQueryable();
}
