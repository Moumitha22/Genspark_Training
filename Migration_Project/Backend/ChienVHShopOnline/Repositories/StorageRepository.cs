using ChienVHShopOnline.Data;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class StorageRepository : Repository<int, Storage>
    {
        public StorageRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Storage> Get(int id)
        {
            var category = await _dbContext.Storages.FindAsync(id);
            return category ?? 
                throw new KeyNotFoundException($"Category with ID {id} not found.");
        }

        public override async Task<IEnumerable<Storage>> GetAll()
        {
            return await _dbContext.Storages
            .OrderBy(c => c.Name)
            .ToListAsync();
        }

    }
}
