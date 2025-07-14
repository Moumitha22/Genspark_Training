using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyRepository : IRepository<Guid, Property>
    {
        Task<PaginatedResult<PropertyResponseDto>> GetByListerIdAsync(Guid agentId, PaginationModel pagination);
        Task<PaginatedResult<PropertyResponseDto>> BasicSearchAsync(BasicPropertySearchModel query, SortModel sort, PaginationModel pagination);
        Task<PaginatedResult<PropertyResponseDto>> AdvancedSearchAsync(AdvancedPropertySearchModel searchModel, SortModel sortModel, PaginationModel paginationModel);
        Task<IEnumerable<Property>> GetSoldProperties();
        Task UpdateStatusAsync(Guid propertyId, string newStatus);

    }
}