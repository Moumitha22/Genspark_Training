namespace OnlineAuctionAPI.Models.DTO;

public class AuctionDeleteRequestDto
{
    public Guid AuctionItemId { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
}
