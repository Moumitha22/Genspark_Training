using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyService
    {
        Task<Property> CreatePropertyAsync(PropertyAddRequestDto dto, Guid agentId);
        Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync();
        Task<PropertyResponseDto> GetPropertyByIdAsync(Guid id);
        Task<IEnumerable<PropertyResponseDto>> GetPropertiesByAgentIdAsync(Guid agentId);
        Task<IEnumerable<PropertyResponseDto>> GetSoldProperties();
        Task<PropertyResponseDto> UpdatePropertyAsync(Guid propertyId, PropertyUpdateRequestDto dto, Guid requesterId, string userRole);
        Task UpdatePropertyStatusAsync(Guid propertyId, string newStatus, Guid requesterId, string userRole);
        Task DeletePropertyAsync(Guid propertyId, Guid requesterId, string userRole);
        Task<IEnumerable<PropertyResponseDto>> SearchPropertiesAsync(PropertySearchModel searchModel, SortModel sortModel);
    }
}