using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionAPI.Models
{
    public class BidItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
        public decimal Amount { get; set; }
        public DateTime BidTime { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid BidderId { get; set; }
        public User? Bidder { get; set; }

        [Required]
        public Guid AuctionItemId { get; set; }
        public AuctionItem? AuctionItem { get; set; }

        [Required]
        public BidStatus Status { get; set; } = BidStatus.Active; 
    }
}
