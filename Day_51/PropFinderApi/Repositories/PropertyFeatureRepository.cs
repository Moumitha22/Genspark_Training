using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models.DTOs;



namespace PropFinderApi.Repositories
{
    public class PropertyFeatureRepository : Repository<Guid, PropertyFeature>, IPropertyFeatureRepository
    {
        public PropertyFeatureRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<PropertyFeature> Get(Guid id)
        {
            var pf = await _propFinderDbContext.PropertyFeatures.FindAsync(id);
            return pf ?? throw new NotFoundException($"Feature for ID '{id}' not found");
        }

        public override async Task<IEnumerable<PropertyFeature>> GetAll()
        {
            return await _propFinderDbContext.PropertyFeatures.ToListAsync();
        }

        public async Task<IEnumerable<PropertyFeature>> GetByPropertyIdAsync(Guid propertyId)
        {
            return await _propFinderDbContext.PropertyFeatures
                .Where(pf => pf.PropertyId == propertyId && !pf.Feature.IsDeleted)
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<PropertyFeature> features)
        {
            await _propFinderDbContext.PropertyFeatures.AddRangeAsync(features);
        }

        public async Task SaveAsync()
        {
            await _propFinderDbContext.SaveChangesAsync();
        }
    }

}