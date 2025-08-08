namespace OnlineAuctionAPI.Interfaces;

using Microsoft.AspNetCore.Mvc;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

public interface IAuctionItemRepository : IRepository<Guid, AuctionItem>
{
    Task<IEnumerable<AuctionItem>> GetActiveAuctionsAsync();

    // Task<AuctionItem?> GetBidsAsync(Guid auctionItemId);
    Task<IEnumerable<AuctionItem>> GetAllAuctionsWithFile();
    Task<AuctionItem?> GetAuctionsWithFile(Guid id);
    Task<(IEnumerable<AuctionItem>, int)> GetPagedItemsAsync(int page, int pageSize);
    public IQueryable<AuctionItem> GetAllQueryable();
    Task<WinnerIdResponseDto> UpdateWinningId(WinningIdUpdateDto dto);

    Task<IEnumerable<AuctionItem>> GetAllEndedAndNotCompletedAsync(DateTime currentTime);
    Task<IEnumerable<AuctionItem>> GetAllUpcomingAndShouldBeLiveAsync(DateTime now);
}
