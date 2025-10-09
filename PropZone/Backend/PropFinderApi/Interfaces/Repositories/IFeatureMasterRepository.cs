using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Interfaces
{
    public interface IFeatureMasterRepository : IRepository<Guid, FeatureMaster>
    {
        Task<IEnumerable<FeatureMaster>> GetApplicableFeaturesAsync(PropertyType propertyType, ListingPurpose listingPurpose);
        Task<IEnumerable<FeatureMaster>> GetApplicableFeaturesByPurposeAsync(ListingPurpose purpose);
        Task UpdateFeatureWithOptionsAsync(Guid featureId, FeatureMasterAddRequestDto dto);
    }
}