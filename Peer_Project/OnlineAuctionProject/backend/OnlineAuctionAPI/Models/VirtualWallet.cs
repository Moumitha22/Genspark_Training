namespace OnlineAuctionAPI.Models;

public class VirtualWallet
{
    public Guid Id { get; set; }
    public decimal Balance { get; set; }
    public Guid UserId { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}