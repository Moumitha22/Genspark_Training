namespace PropFinderApi.Models.DTOs
{
    public class FeatureFieldDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string FilterMode { get; set; } = string.Empty;
        public List<FeatureOptionDto> Options { get; set; }
    }
    
}