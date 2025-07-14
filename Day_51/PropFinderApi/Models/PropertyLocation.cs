namespace PropFinderApi.Models
{
    public class PropertyLocation
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }

        public string Locality { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Property Property { get; set; } = null!;
    }
    
}