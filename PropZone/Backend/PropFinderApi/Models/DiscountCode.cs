namespace PropFinderApi.Models
{
    public class DiscountCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsActive { get; set; }
        public List<DiscountCodeOptions> Options { get; set; } = new List<DiscountCodeOptions>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public int? MaxListerLimit { get; set; }
        public int ListerUsageCount { get; set; }

        // Navigation properties
        public ICollection<PropertyDiscountCode> PropertyDiscountCodes { get; set; } =
            new List<PropertyDiscountCode>();
    }
}
