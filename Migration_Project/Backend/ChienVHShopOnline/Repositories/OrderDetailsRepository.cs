using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories
{
    public class OrderDetailRepository : Repository<(int OrderId, int ProductId), OrderDetail>
    {
        public OrderDetailRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<OrderDetail> Get((int OrderId, int ProductId) key)
        {
            return await _dbContext.OrderDetails
                .Include(od => od.Product) 
                .Include(od => od.Order)  
                .FirstOrDefaultAsync(od => od.OrderId == key.OrderId && od.ProductId == key.ProductId);
        }

        public override async Task<IEnumerable<OrderDetail>> GetAll()
        {
            return await _dbContext.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .ToListAsync();
        }
    }
}
