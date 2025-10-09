namespace PropFinderApi.Models.DTOs
{
    public class PropertyFeatureResponseDto
    {
        public Guid FeatureId { get; set; }
        public string FeatureName { get; set; } = string.Empty;
        public List<string> Values { get; set; } = new();
        public Guid? OptionId { get; set; }
    }

}