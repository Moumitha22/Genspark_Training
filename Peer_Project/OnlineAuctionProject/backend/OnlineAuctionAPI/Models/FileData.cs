namespace OnlineAuctionAPI.Models;

public class FileData
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public byte[]? Data { get; set; }
    public Guid AuctionItemId { get; set; }
    public AuctionItem? AuctionItem { get; set; }
    public string BlobUrl { get; set; } = string.Empty;
}
