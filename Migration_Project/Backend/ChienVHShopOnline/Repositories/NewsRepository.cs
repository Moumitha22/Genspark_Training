using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class NewsRepository : Repository<int, News>
    {
        public NewsRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<News> Get(int key)
        {
            var news = await _dbContext.News
                .FirstOrDefaultAsync(c => c.Id == key);
            return news ?? throw new KeyNotFoundException($"News with ID {key} not found.");
        }

        public override async Task<IEnumerable<News>> GetAll()
        {
            return await _dbContext.News
                .ToListAsync();
        }
    }
}
