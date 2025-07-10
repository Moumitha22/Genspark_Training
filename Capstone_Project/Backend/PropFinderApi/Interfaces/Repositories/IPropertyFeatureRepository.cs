using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyFeatureRepository : IRepository<Guid, PropertyFeature>
    {
        Task<IEnumerable<PropertyFeature>> GetByPropertyIdAsync(Guid propertyId);
        Task AddRangeAsync(IEnumerable<PropertyFeature> features);
        Task SaveAsync();
    }


}