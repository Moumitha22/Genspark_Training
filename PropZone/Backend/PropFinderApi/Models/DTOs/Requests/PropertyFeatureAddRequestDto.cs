namespace PropFinderApi.Models.DTOs
{
    public class PropertyFeatureAddRequestDto
    {
        public Guid FeatureId { get; set; }
        public string? Value { get; set; }
        public Guid? OptionId { get; set; }
        public string DataType { get; set; } = string.Empty;  // "Text", "Dropdown", etc.
    }

}