using Microsoft.AspNetCore.Mvc;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IAuctionItemService
{
    Task<AuctionItemResponseDto> AddAuctionItemAsync(AuctionItemAddDto auctionDto);

    Task<AuctionItemResponseDto?> GetAuctionItemByIdAsync(Guid id);
    Task<IEnumerable<AuctionItemResponseDto>> GetAllAuctionItemAsync();
    Task<PaginatedResponseDto<AuctionItemResponseDto>> GetPagedAuctionItemsAsync(
        PaginationDto pagination
    );
    Task<AuctionItemResponseDto?> UpdateAuctionItemAsync(Guid id, AuctionItemAddDto updateDto);
    Task<WinnerIdResponseDto> UpdateWinningId(WinningIdUpdateDto winningIdUpdateDto);
    Task<IEnumerable<AuctionItemResponseDto>> GetActiveAuctionsAsync();
    Task<AuctionItemResponseDto> UpdateAuctionStatusAsync(
        Guid auctionItemId,
        AuctionStatus newStatus
    );
    Task<WinnerIdResponseDto> GetAuctionWinnerAsync(Guid auctionItemId);
    Task<IEnumerable<AuctionItemResponseDto>> GetAllAuctionItemBySellerAsync(Guid sellerId);
    Task<decimal> GetTotalSales(Guid sellerId);
    Task<bool> DeleteAuctionItemAsync(Guid id);
}
