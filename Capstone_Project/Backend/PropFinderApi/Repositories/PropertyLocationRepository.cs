using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Repositories
{
    public class PropertyLocationRepository : Repository<Guid, PropertyLocation>, IPropertyLocationRepository
    {
        public PropertyLocationRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<PropertyLocation> Get(Guid id)
        {
            var loc = await _propFinderDbContext.PropertyLocations.FindAsync(id);
            return loc ?? throw new NotFoundException($"Location with ID '{id}' not found");
        }

        public override async Task<IEnumerable<PropertyLocation>> GetAll()
        {
            return await _propFinderDbContext.PropertyLocations.ToListAsync();
        }

        public async Task<PropertyLocation?> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _propFinderDbContext.PropertyLocations
                .FirstOrDefaultAsync(l => l.PropertyId == propertyId);
        }


        public async Task UpsertAsync(Guid propertyId, PropertyLocationAddRequestDto dto)
        {

            var existing = await _propFinderDbContext.PropertyLocations
                .FirstOrDefaultAsync(l => l.PropertyId == propertyId);

            if (existing == null)
            {
                var newLocation = new PropertyLocation
                {
                    Id = Guid.NewGuid(),
                    PropertyId = propertyId,
                    Locality = dto.Locality,
                    City = dto.City,
                    State = dto.State,
                    Latitude = dto.Latitude,
                    Longitude = dto.Longitude,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _propFinderDbContext.PropertyLocations.AddAsync(newLocation);
            }
            else
            {
                existing.Locality = dto.Locality;
                existing.City = dto.City;
                existing.State = dto.State;
                existing.Latitude = dto.Latitude;
                existing.Longitude = dto.Longitude;
                existing.UpdatedAt = DateTime.UtcNow;
            }

            await _propFinderDbContext.SaveChangesAsync();
        }
    }

    
}