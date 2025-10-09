using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyResponseDto> CreatePropertyAsync(PropertyAddRequestDto dto, Guid listerId);
        Task<IEnumerable<PropertyResponseDto>> GetAllPropertiesAsync();
        Task<PropertyResponseDto> GetPropertyByIdAsync(Guid id);
        Task<PaginatedResult<PropertyResponseDto>> GetPropertiesByListerIdAsync(Guid listerId, PaginationModel paginationModel);
        Task<PropertyResponseDto> UpdatePropertyAsync(Guid propertyId, PropertyAddRequestDto dto, Guid requesterId, string userRole);
        Task UpdatePropertyStatusAsync(Guid propertyId, string newStatus, Guid requesterId, string userRole);
        Task SoftDeletePropertyAsync(Guid propertyId);
        Task<PaginatedResult<PropertyResponseDto>> BasicSearchPropertiesAsync(BasicPropertySearchModel searchModel,SortModel sortModel,PaginationModel paginationModel);
        Task<PaginatedResult<PropertyResponseDto>> AdvancedSearchPropertiesAsync(AdvancedPropertySearchModel searchModel, SortModel sortModel, PaginationModel paginationModel);
    }
}