namespace PropFinderApi.Models.DTOs
{
    public class FeatureAdminDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string FilterMode { get; set; } = string.Empty;
    public List<FeatureOptionDto> Options { get; set; } = new();
    public List<FeatureApplicabilityDto> Applicability { get; set; } = new();
} 
    
}