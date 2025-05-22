using OrderProcessingApplication.Interfaces;
using OrderProcessingApplication.Models;
using OrderProcessingApplication.Exceptions;
using System.Collections.Generic;

namespace OrderProcessingApplication.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<int, Order> _orderRepository;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly INotifier _notifier;

        public OrderService(IRepository<int, Order> orderRepository, IPaymentProcessor paymentProcessor, INotifier notifier)
        {
            _orderRepository = orderRepository;
            _paymentProcessor = paymentProcessor;
            _notifier = notifier;
        }

        // Place an order: Add order, process payment, send notification
        public Order PlaceOrder(List<OrderItem> items)
        {
            var order = new Order {
                Items = items 
            };

            _orderRepository.Add(order);

            if (!_paymentProcessor.ProcessPayment(order.TotalAmount))
            {
                throw new PaymentFailedException("Payment failed for the order.");
            }

            _notifier.SendNotification($"Order with ID {order.Id} has been placed successfully.");

            return order;
        }

        public Order GetOrder(int id)
        {
            return _orderRepository.GetById(id);
        }

        public ICollection<Order> GetAllOrders()
        {
            return _orderRepository.GetAll();
        }
    }
}
