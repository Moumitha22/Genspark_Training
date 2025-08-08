
namespace OnlineAuctionAPI.Models.DTO;

public class WinnerIdResponseDto
{
    public Guid AuctionItemId { get; set; }
    public Guid WinnerId { get; set; }
    public string WinnerName { get; set; } = string.Empty;
    public decimal WinningPrice { get; set; }
}