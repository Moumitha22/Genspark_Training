using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class Property
    {
        public Guid Id { get; set; }
        public Guid ListerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PropertyType PropertyType { get; set; }
        public ListingPurpose ListingPurpose { get; set; }
        public ListerType ListerType { get; set; }
        public ListingStatus Status { get; set; }
        public decimal Price { get; set; }
        public decimal AreaSqFt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }

        // Navigation
        public User Lister { get; set; } = null!;
        public PropertyLocation Location { get; set; } = null!;
        public ICollection<PropertyImage>? PropertyImages { get; set; }
        public ICollection<PropertyFeature>? Features { get; set; }
        public ICollection<ContactLog>? ContactRequests { get; set; }
        public ICollection<PropertyDiscountCode> PropertyDiscountCodes { get; set; } =
            new List<PropertyDiscountCode>();
    }
}
