namespace OnlineAuctionAPI.Models;

public class EAgreement
{
    public Guid Id { get; set; }
    public Guid AuctionItemId { get; set; }
    public Guid BiddingId { get; set; }
    public byte[]? File { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public AuctionItem? AuctionItem { get; set; }
    public BidItem? Bidding { get; set; }

    // public int StatusId { get; set; }
    // public Status? DeletionStatus { get; set; }
}
