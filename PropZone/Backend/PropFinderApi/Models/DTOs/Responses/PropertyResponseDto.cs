namespace PropFinderApi.Models.DTOs
{
    public class PropertyResponseDto
    {
        public Guid Id { get; set; }
        public Guid ListerId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public PropertyLocationResponseDto Location { get; set; } = new();
        public string PropertyType { get; set; } = string.Empty;
        public string ListingPurpose { get; set; } = string.Empty;
        public string ListerType { get; set; } = string.Empty;
        public decimal AreaSqFt { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Available";

        public List<string> ImageUrls { get; set; } = new();

        public List<PropertyFeatureResponseDto> FeatureSummary { get; set; } = new();

        public List<DiscountCodeDto> DiscountCodes { get; set; } = new();
    }
}
