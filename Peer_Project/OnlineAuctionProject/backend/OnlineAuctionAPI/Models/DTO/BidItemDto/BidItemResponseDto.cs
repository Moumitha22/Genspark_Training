
namespace OnlineAuctionAPI.Models.DTO;

public class BidItemResponseDto
{
    public Guid Id { get; set; }
    public Guid AuctionItemId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime BidTime { get; set; }
    public string? AuctionName { get; set; }
}