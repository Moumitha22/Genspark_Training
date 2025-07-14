using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Interfaces
{
    public interface IFeatureMasterService
    {
        Task<FeatureFieldDto> CreateFeatureAsync(FeatureMasterAddRequestDto dto);

        Task<FeatureMaster> GetAsync( Guid featureId);

        Task<IEnumerable<FeatureAdminDto>> GetAllFeaturesAsync();

        Task<FeatureFieldDto> UpdateFeatureAsync(Guid featureId, FeatureMasterAddRequestDto dto);

        Task<bool> SoftDeleteFeatureAsync(Guid featureId);
        
        Task<IEnumerable<FeatureFieldDto>> GetApplicableFeaturesAsync(PropertyType propertyType, ListingPurpose listingPurpose);

        Task<IEnumerable<FeatureFieldDto>> GetApplicableFeaturesByPurposeAsync(ListingPurpose purpose);

    }
}
