using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyFeatureService
    {
        Task UpdateFeatureSetAsync(Guid propertyId, List<PropertyFeatureAddRequestDto> dtos);
    }

}