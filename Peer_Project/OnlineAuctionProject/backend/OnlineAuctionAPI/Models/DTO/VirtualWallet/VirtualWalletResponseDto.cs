namespace OnlineAuctionAPI.Models.DTO;

public class VirtualWalletResponseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal Balance { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}