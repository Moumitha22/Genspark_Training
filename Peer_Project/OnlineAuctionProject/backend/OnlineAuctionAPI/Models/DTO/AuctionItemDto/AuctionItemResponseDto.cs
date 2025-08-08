namespace OnlineAuctionAPI.Models.DTO;

public class AuctionItemResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal StartingPrice { get; set; }
    public decimal ReservePrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid SellerId { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string? WinnerId { get; set; }
    public List<FileDataDto> Files { get; set; } = new();
    public DateTime CreatedAt { get; set; }

    public AuctionDeleteRequest? DeleteRequest { get; set; }
}
