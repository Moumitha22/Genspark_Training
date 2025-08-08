namespace OnlineAuctionAPI.Models;

public class AuctionDeleteRequest
{
    public Guid Id { get; set; }
    public Guid AuctionItemId { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    public bool IsApproved { get; set; } = false;
    public string? Remarks { get; set; } = null;
    public DateTime? ApprovalDate { get; set; } = null;
}
