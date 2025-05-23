using OrderProcessingApplication.Interfaces;
using OrderProcessingApplication.Models;
using OrderProcessingApplication.Repositories;
using OrderProcessingApplication.Services;
using System;

namespace OrderProcessingApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IRepository<int, Order> orderRepository = new OrderRepository();
            IPaymentProcessor paymentProcessor = new PaymentProcessor();
            IEmailNotifier notifier = new EmailNotifier();

            IOrderService orderService = new OrderService(orderRepository, paymentProcessor, notifier);

            ManageOrders manageOrders = new ManageOrders(orderService);

            manageOrders.Run();
        }
    }
}
