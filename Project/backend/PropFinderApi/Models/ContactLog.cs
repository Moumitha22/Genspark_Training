namespace PropFinderApi.Models
{
    public class ContactLog
    {
        public Guid Id { get; set; }

        public Guid PropertyId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid AgentId { get; set; }
        public string Message { get; set; } = string.Empty;
        public string BuyerPhoneNumber { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Property Property { get; set; } = null!;
        public User Buyer { get; set; } = null!;
        public User Agent { get; set; } = null!;
    }
}
