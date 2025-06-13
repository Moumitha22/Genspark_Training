using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyRepository : IRepository<Guid, Property>
    {
        Task<IEnumerable<Property>> GetByAgentIdAsync(Guid agentId, bool includeSoldOrDeleted = false);
        Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchModel searchModel, SortModel sortModel);
        Task<IEnumerable<Property>> GetSoldProperties();
        Task UpdateStatusAsync(Guid propertyId, string newStatus);

    }
}