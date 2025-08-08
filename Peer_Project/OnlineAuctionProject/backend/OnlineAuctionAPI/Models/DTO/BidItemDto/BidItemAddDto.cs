namespace OnlineAuctionAPI.Models.DTO;

public class BidItemAddDto
{
    public Guid BidderId { get; set; }
    public Guid AuctionItemId { get; set; }   
    public decimal Amount { get; set; }
}