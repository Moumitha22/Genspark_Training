using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyRepository : IRepository<Guid, Property>
    {
        Task<PaginatedResult<Property>> GetByListerIdAsync(Guid agentId, PaginationModel pagination);
        Task<PaginatedResult<Property>> BasicSearchAsync(BasicPropertySearchModel query, SortModel sort, PaginationModel pagination);
        Task<PaginatedResult<Property>> AdvancedSearchAsync(AdvancedPropertySearchModel searchModel, SortModel sortModel, PaginationModel paginationModel);
        Task UpdateStatusAsync(Guid propertyId, string newStatus);

    }
}