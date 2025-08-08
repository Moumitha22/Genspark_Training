using ChienVHShopOnline.Data;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class CategoryRepository : Repository<int, Category>
    {
        public CategoryRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Category> Get(int id)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            return category ?? 
                throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        public override async Task<IEnumerable<Category>> GetAll()
        {
            return await _dbContext.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
        }

    }
}
