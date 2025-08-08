using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Interfaces;

public class AuctionItemAddDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal ReservePrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public List<IFormFile>? FileAttachments { get; set; }
}
