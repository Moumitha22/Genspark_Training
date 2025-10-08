using Microsoft.EntityFrameworkCore;
using PropFinderApi.Models;
using PropFinderApi.Contexts;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Repositories
{
    public class PropertyRepository : Repository<Guid, Property>, IPropertyRepository
    {
        public PropertyRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<IEnumerable<Property>> GetAll()
        {
            return await _propFinderDbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => !p.IsDeleted && p.Status == "Available")
                .ToListAsync();
        }
        public override async Task<Property> Get(Guid id)
        {
            var property = await _propFinderDbContext.Properties
                .Include(p => p.Agent)
                .Include(p => p.PropertyImages)
                .SingleOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            return property ?? throw new NotFoundException($"Property with ID '{id}' not found");
        }

        public async Task<IEnumerable<Property>> GetSoldProperties()
        {
            return await _propFinderDbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => !p.IsDeleted && p.Status == "Sold")
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetByAgentIdAsync(Guid agentId, bool includeSoldOrDeleted = false)
        {
            var query = _propFinderDbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => p.AgentId == agentId);

            if (!includeSoldOrDeleted)
            {
                query = query.Where(p => !p.IsDeleted);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateStatusAsync(Guid propertyId, string newStatus)
        {
            var property = await _propFinderDbContext.Properties.FindAsync(propertyId);
            if (property == null)
                throw new NotFoundException("Property not found");

            property.Status = newStatus;
            property.UpdatedAt = DateTime.UtcNow;

            _propFinderDbContext.Properties.Update(property);
            await _propFinderDbContext.SaveChangesAsync();
        }


        public async Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchModel searchModel, SortModel sortModel)
        {
            var query = _propFinderDbContext.Properties
                .Include(p => p.PropertyImages)
                .Where(p => !p.IsDeleted && p.Status == "Available");

            query = ApplyFilters(query, searchModel);
            query = ApplySorting(query, sortModel);

            return await query.ToListAsync(); // Executes DB query here
        }

        private IQueryable<Property> ApplyFilters(IQueryable<Property> query, PropertySearchModel searchModel)
        {
            if (searchModel.ListingType.HasValue)
                query = query.Where(p => p.ListingType == searchModel.ListingType.Value);

            if (searchModel.PropertyType.HasValue)
                query = query.Where(p => p.PropertyType == searchModel.PropertyType.Value);

            if (searchModel.AgentId.HasValue)
                query = query.Where(p => p.AgentId == searchModel.AgentId.Value);

            if (searchModel.PriceRange?.Min.HasValue == true)
                query = query.Where(p => p.Price >= searchModel.PriceRange.Min.Value);

            if (searchModel.PriceRange?.Max.HasValue == true)
                query = query.Where(p => p.Price <= searchModel.PriceRange.Max.Value);

            if (searchModel.BedroomsRange?.Min.HasValue == true)
                query = query.Where(p => p.Bedrooms >= searchModel.BedroomsRange.Min.Value);

            if (searchModel.BedroomsRange?.Max.HasValue == true)
                query = query.Where(p => p.Bedrooms <= searchModel.BedroomsRange.Max.Value);

            if (searchModel.BathroomsRange?.Min.HasValue == true)
                query = query.Where(p => p.Bathrooms >= searchModel.BathroomsRange.Min.Value);

            if (searchModel.BathroomsRange?.Max.HasValue == true)
                query = query.Where(p => p.Bathrooms <= searchModel.BathroomsRange.Max.Value);

            if (searchModel.IsFurnished.HasValue)
                query = query.Where(p => p.IsFurnished == searchModel.IsFurnished.Value);

            if (searchModel.HasParking.HasValue)
                query = query.Where(p => p.HasParking == searchModel.HasParking.Value);

            if (searchModel.HasBalcony.HasValue)
                query = query.Where(p => p.HasBalcony == searchModel.HasBalcony.Value);

            if (!string.IsNullOrEmpty(searchModel.Title))
                query = query.Where(p => EF.Functions.ILike(p.Title, $"%{searchModel.Title}%"));

            if (!string.IsNullOrEmpty(searchModel.Location))
                query = query.Where(p => EF.Functions.ILike(p.Location, $"%{searchModel.Location}%"));

            if (!string.IsNullOrEmpty(searchModel.Description))
                query = query.Where(p => EF.Functions.ILike(p.Description, $"%{searchModel.Description}%"));

            return query;
        }

        private IQueryable<Property> ApplySorting(IQueryable<Property> query, SortModel sortModel)
        {
            var validSortFields = new HashSet<string> { "Price", "AreaSqFt", "Bedrooms", "Bathrooms", "CreatedAt" };

            if (validSortFields.Contains(sortModel.SortBy))
            {
                query = sortModel.Ascending
                    ? query.OrderBy(p => EF.Property<object>(p, sortModel.SortBy))
                    : query.OrderByDescending(p => EF.Property<object>(p, sortModel.SortBy));
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt); // Default sorting
            }

            return query;
        }
    }
}