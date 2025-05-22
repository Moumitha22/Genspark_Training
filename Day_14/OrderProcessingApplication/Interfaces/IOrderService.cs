using OrderProcessingApplication.Models;

namespace OrderProcessingApplication.Interfaces
{
    public interface IOrderService
    {
        Order PlaceOrder(List<OrderItem> items);
        Order GetOrder(int id);
        ICollection<Order> GetAllOrders();
    }
}
