using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ModelRepository : Repository<int, ChienVHShopOnline.Models.Model>
    {
        public ModelRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Model> Get(int key)
        {
            var model = await _dbContext.Models
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == key);
            return model ?? throw new KeyNotFoundException($"Model with ID {key} not found.");
        }

        public override async Task<IEnumerable<Model>> GetAll()
        {
            return await _dbContext.Models
                .Include(c => c.Products) 
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
