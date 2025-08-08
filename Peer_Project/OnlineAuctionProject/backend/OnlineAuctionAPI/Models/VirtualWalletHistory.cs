namespace OnlineAuctionAPI.Models;
public class VirtualWalletHistory
{
    public Guid Id { get; set; }
    public Guid VirtualWalletId { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public VirtualWallet? VirtualWallet { get; set; }
}