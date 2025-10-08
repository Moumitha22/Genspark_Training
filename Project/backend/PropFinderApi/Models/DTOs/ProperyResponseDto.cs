namespace PropFinderApi.Models.DTOs
{
    public class PropertyResponseDto
    {
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Location { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string ListingType { get; set; } = string.Empty;
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public decimal AreaSqFt { get; set; }
        public bool IsFurnished { get; set; } = false;
        public bool HasParking { get; set; } = false;
        public bool HasBalcony { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; } = "Available";

        public List<string> ImageUrls { get; set; } = new(); 
    }
}