using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models
{
    public class FeatureApplicability
{
    public Guid Id { get; set; }
    public Guid FeatureId { get; set; }

    public PropertyType AppliesToType { get; set; }    // Apartment, House, Plot, CommercialSpace
    public ListingPurpose AppliesToPurpose { get; set; }  // Enum: Sale, Rent
    public bool IsDeleted { get; set; }

    // Navigation
        public FeatureMaster Feature { get; set; } = null!;
}
}