
using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionAPI.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = null!;

        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid user role. Please give a valid role like Seller, Bidder, or Both.")]
        public UserRole Role { get; set; }
        public ICollection<AuctionItem>? Auctions { get; set; }
        public ICollection<BidItem>? Bids { get; set; }
        public int StatusId { get; set; }
        public Status? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public VirtualWallet? VirtualWallet { get; set; }
        
    }
}