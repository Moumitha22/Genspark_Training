namespace OnlineAuctionAPI.Models;

public class WinningIdUpdateDto
{
    public Guid WinningId { get; set; }
    public Guid AuctionItemId { get; set; }
}