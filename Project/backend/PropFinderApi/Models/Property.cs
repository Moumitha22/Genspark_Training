using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public ListingType ListingType { get; set; }
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public decimal AreaSqFt { get; set; }
        public bool IsFurnished { get; set; } = false;
        public bool HasParking { get; set; } = false;
        public bool HasBalcony { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = "Available";
        public bool IsDeleted { get; set; }

        // Navigation
        public User Agent { get; set; } = null!;
        public ICollection<PropertyImage>? PropertyImages { get; set; }
        public ICollection<ContactLog>? ContactRequests { get; set; }
    }
}