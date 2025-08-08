namespace OnlineAuctionAPI.Models.DTO;

public class AuctionDeleteRequestRejectDto
{
    public Guid AuctionItemId { get; set; }
    public string Remarks { get; set; } = string.Empty;
}
