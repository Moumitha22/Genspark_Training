using ChienVHShopOnline.Data;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ColorRepository : Repository<int, Color>
    {
        public ColorRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<Color> Get(int id)
        {
            var color = await _dbContext.Colors.FindAsync(id);
            return color ?? throw new KeyNotFoundException($"Color with ID {id} not found.");
        }

        public override async Task<IEnumerable<Color>> GetAll()
        {
            return await _dbContext.Colors.OrderBy(c => c.Name).ToListAsync();
        }
    }
}
