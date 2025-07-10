using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IPropertyLocationRepository : IRepository<Guid, PropertyLocation>
    {
        Task<PropertyLocation?> GetByPropertyIdAsync(Guid propertyId);

        Task UpsertAsync(Guid propertyId, PropertyLocationAddRequestDto locationDto);
    }
}