using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public AgentProfile? AgentProfile { get; set; } // only for agents
        public ICollection<Property>? Properties { get; set; } // Agent's properties
        public ICollection<ContactLog>? ContactRequestsMade { get; set; } // Buyer inquiries
        public ICollection<ContactLog>? ContactRequestsReceived { get; set; } // Agent inquiries
    }
}