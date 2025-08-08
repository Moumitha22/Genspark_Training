using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Interfaces
{
    public interface IOrderRepository : IRepository<int, Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserId(int userId);
    }
}
