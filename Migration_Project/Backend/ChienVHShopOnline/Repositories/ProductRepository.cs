using ChienVHShopOnline.Data;
using ChienVHShopOnline.Interfaces;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class ProductRepository : Repository<int, Product>, IProductRepository
    {
        public ProductRepository(AppDbContext dbContext) : base(dbContext) { }

        public override async Task<Product> Get(int id)
        {
            var product = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Model)
                .Include(p => p.Storage)
                .FirstOrDefaultAsync(p => p.Id == id);

            return product ?? throw new KeyNotFoundException($"Product with ID {id} not found.");
        }

        public override async Task<IEnumerable<Product>> GetAll()
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Model)
                .Include(p => p.Storage)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedProducts(int pageNumber, int pageSize)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Model)
                .Include(p => p.Storage)
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedProductsByCategory(int categoryId, int pageNumber, int pageSize)
        {
            return await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Model)
                .Include(p => p.Storage)
                .Where(p => p.CategoryId == categoryId)
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedProductsByUserId(int userId, int pageNumber, int pageSize)
        {
            return await _dbContext.Products
                .Where(p => p.UserId == userId)
                .Include(p => p.Category)
                .Include(p => p.Color)
                .Include(p => p.Model)
                .Include(p => p.Storage)
                .OrderByDescending(p => p.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        
    }
}
