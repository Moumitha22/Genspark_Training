namespace PropFinderApi.Models
{
    public class ContactLog
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid ListerId { get; set; }

        public string BuyerPhoneNumber { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public string ListerName { get; set; } = string.Empty;
        public string ListerPhoneNumber { get; set; } = string.Empty;
        public string ListerEmail { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Property Property { get; set; } = null!;
        public User Buyer { get; set; } = null!;
        public User Lister { get; set; } = null!;
    }
}
