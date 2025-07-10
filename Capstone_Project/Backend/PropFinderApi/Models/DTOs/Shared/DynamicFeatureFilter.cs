using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class DynamicFeatureFilter
    {
        public Guid FeatureId { get; set; }                                   // From FeatureMaster
        public List<string> Values { get; set; } = new();                     // Match against Value or Option.Value
        public FeatureFilterMode FilterMode { get; set; } = FeatureFilterMode.Exact;
    }
}