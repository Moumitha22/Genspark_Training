using System.ComponentModel.DataAnnotations;

namespace OnlineAuctionAPI.Models
{
    public class AuctionItem
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Starting price must be greater than zero.")]
        public decimal StartingPrice { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Reserve price must be greater than zero.")]
        public decimal ReservePrice { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        [Required]
        [EnumDataType(typeof(AuctionStatus), ErrorMessage = "Invalid auction status.")]
        public AuctionStatus Status { get; set; }

        // [Required]
        // public string? Category { get; set; }

        [Required]
        public Guid SellerId { get; set; }
        public User? Seller { get; set; } = null!;

        public Guid? WinnerId { get; set; }
        public BidItem? WinningBid { get; set; }
        public ICollection<BidItem> Bids { get; set; } = new List<BidItem>();
        public ICollection<FileData>? FileAttachments { get; set; } = new List<FileData>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public AuctionDeleteRequest? DeleteRequest { get; set; } = null;
    }
}
