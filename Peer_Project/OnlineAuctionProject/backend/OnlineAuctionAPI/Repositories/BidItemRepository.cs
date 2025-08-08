using Microsoft.EntityFrameworkCore;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Repositories;

public class BidItemRepository : Repository<Guid, BidItem>, IBidItemRepository
{
    public BidItemRepository(AuctionContext context)
        : base(context) { }

    public async Task<IEnumerable<BidItem>> GetBidsByAuctionAsync(Guid auctionItemId, bool includeArchived = false)
    {
        try
        {
            var query = _auctionContext.BidItems
                .Where(b => b.AuctionItemId == auctionItemId);

            if (!includeArchived)
            {
                query = query.Where(b => b.Status != BidStatus.Archived);
            }

            var bids = await query
                .Include(b => b.Bidder)
                .OrderByDescending(b => b.Amount)
                .ToListAsync();
            return bids;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Getting bids by auctionId", ex);
        }
    }

    public async Task<BidItem?> GetHighestBidAsync(Guid auctionItemId)
    {
        try
        {
            var highestBid = await _auctionContext
                .BidItems.Where(b => b.AuctionItemId == auctionItemId)
                .OrderByDescending(b => b.Amount)
                .ThenBy(b => b.BidTime)
                .FirstOrDefaultAsync();
            return highestBid;
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Getting Highest bid in the auction", ex);
        }
    }

    public async Task<IEnumerable<BidItem>> GetBidsByUserIdAsync(Guid userId)
    {
        try
        {
            return await _auctionContext.BidItems
            .Include(b => b.AuctionItem)
            .Where(b => b.BidderId == userId && b.Status != BidStatus.Archived).ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Error retrieving bids by user ID.", ex);
        }
    }

    public async Task<BidItem?> GetByIdAsync(Guid bidId)
    {
        try
        {
            return await _auctionContext.BidItems.FirstOrDefaultAsync(b => b.Id == bidId);
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Error retrieving bid by ID.", ex);
        }
    }

    public async Task<decimal?> GetMaxBidAmountAsync(Guid auctionItemId, DateTime endTime)
    {
        try
        {
            return await _auctionContext
                .BidItems.Where(b => b.AuctionItemId == auctionItemId && b.BidTime <= endTime)
                .MaxAsync(b => (decimal?)b.Amount);
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Error retrieving max bid amount.", ex);
        }
    }

    public async Task<List<BidItem>> GetHighestBidsAsync(
        Guid auctionItemId,
        DateTime endTime,
        decimal maxAmount
    )
    {
        try
        {
            return await _auctionContext
                .BidItems.Where(b =>
                    b.AuctionItemId == auctionItemId
                    && b.BidTime <= endTime
                    && b.Amount == maxAmount
                )
                .OrderBy(b => b.BidTime)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new RepositoryOperationException("Error retrieving highest bids.", ex);
        }
    }
    
    public IQueryable<BidItem> GetAllQueryable()
    {
        return _auctionContext.BidItems.Include(b => b.AuctionItem).AsQueryable();
    }
}
