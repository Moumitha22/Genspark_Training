using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class FeatureApplicabilityDto
{
    public PropertyType AppliesToType { get; set; }
    public ListingPurpose AppliesToPurpose { get; set; }
}
}