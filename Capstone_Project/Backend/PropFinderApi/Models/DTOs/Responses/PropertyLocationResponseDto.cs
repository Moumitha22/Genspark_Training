namespace PropFinderApi.Models.DTOs
{
    public class PropertyLocationResponseDto
    {
        public string Locality { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}
