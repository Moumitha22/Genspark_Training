namespace PropFinderApi.Models
{
    public class AgentProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? LicenseNumber { get; set; } = string.Empty;
        public string? AgencyName { get; set; }
        public string BusinessPhoneNumber { get; set; } = string.Empty;
        public bool IsVerifiedAgent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public User User { get; set; } = null!;
    }
}