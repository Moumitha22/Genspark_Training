using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class FeatureMasterAddRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public FeatureDataType DataType { get; set; }
        public FeatureFilterMode FilterMode { get; set; }

        public List<string>? Options { get; set; } // Only if Dropdown or MultiSelect

        public List<FeatureApplicabilityDto> Applicabilities { get; set; } = new();
    }
}