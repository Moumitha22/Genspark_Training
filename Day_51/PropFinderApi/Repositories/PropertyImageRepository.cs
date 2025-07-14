using PropFinderApi.Contexts;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Exceptions;

namespace PropFinderApi.Repositories
{
    public class PropertyImageRepository : Repository<Guid, PropertyImage>
    {
        public PropertyImageRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<PropertyImage> Get(Guid key)
        {
            var image = await _propFinderDbContext.PropertyImages.SingleOrDefaultAsync(i => i.Id == key);
            return image ?? throw new NotFoundException($"Property image with ID {key} not found");
        }

        public override async Task<IEnumerable<PropertyImage>> GetAll()
        {
            return await _propFinderDbContext.PropertyImages.ToListAsync();
        }
    }
}
