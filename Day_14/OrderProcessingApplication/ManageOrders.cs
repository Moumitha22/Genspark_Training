using OrderProcessingApplication.Interfaces;
using OrderProcessingApplication.Models;
using OrderProcessingApplication.Services;
using System;
using System.Collections.Generic;

namespace OrderProcessingApplication
{
    public class ManageOrders
    {
        private readonly IOrderService _orderService;

        public ManageOrders(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public void Run()
        {
            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        PlaceOrder();
                        break;
                    case "2":
                        ViewAllOrders();
                        break;
                    case "3":
                        ViewOrderById();
                        break;
                    case "4":
                        ExitApplication();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private void ShowMainMenu()
        {
            Console.WriteLine("\n====== Order Management System ======");
            Console.WriteLine("1. Place an Order");
            Console.WriteLine("2. View All Orders");
            Console.WriteLine("3. View Order by ID");
            Console.WriteLine("4. Exit");
            Console.Write("\nChoose an option: ");
        }

        private void PlaceOrder()
        {
            int itemCount = GetItemCount();

            var items = GetOrderItems(itemCount);

            try
            {
                var order = _orderService.PlaceOrder(items);
                Console.WriteLine($"\nOrder placed successfully! Order ID: {order.Id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private int GetItemCount()
        {
            Console.Write("\nPlease enter the number of items in the order:");
            int itemCount;
            while (!int.TryParse(Console.ReadLine(), out itemCount) || itemCount <= 0)
            {
                Console.Write("Invalid number of items. Please enter a valid number: ");
            }
            return itemCount;
        }

        private List<OrderItem> GetOrderItems(int itemCount)
        {
            var items = new List<OrderItem>();

            for (int i = 0; i < itemCount; i++)
            {
                Console.WriteLine($"\nEnter details for item {i + 1}:");
                var item = GetOrderItemDetails();
                items.Add(item);
            }

            return items;
        }

        private OrderItem GetOrderItemDetails()
        {
            Console.Write("Product Name: ");
            string productName = Console.ReadLine();
            while (string.IsNullOrEmpty(productName))
            {
                Console.Write("Product name can't be empty. Please enter a product name: ");
                productName = Console.ReadLine();
            }

            int quantity = GetPositiveInteger("Quantity: ");
            double unitPrice = GetPositiveDouble("Unit Price: ");

            return new OrderItem
            {
                ProductName = productName,
                Quantity = quantity,
                UnitPrice = unitPrice
            };
        }

        private int GetPositiveInteger(string prompt)
        {
            int value;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.Write("Invalid input. Please enter a valid positive number: ");
            }
            return value;
        }

        private double GetPositiveDouble(string prompt)
        {
            double value;
            Console.Write(prompt);
            while (!double.TryParse(Console.ReadLine(), out value) || value <= 0)
            {
                Console.Write("Invalid input. Please enter a valid positive price: ");
            }
            return value;
        }

        private void ViewAllOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders();
                DisplayOrders(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void DisplayOrders(IEnumerable<Order> orders)
        {
            Console.WriteLine("\nAll Orders:");
            foreach (var order in orders)
            {
                Console.WriteLine($"Order ID: {order.Id}, Total Amount: {order.TotalAmount}");
            }
        }

        private void ViewOrderById()
        {
            int orderId = GetOrderIdFromUser();

            try
            {
                var order = _orderService.GetOrder(orderId);
                DisplayOrderDetails(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private int GetOrderIdFromUser()
        {
            Console.Write("Enter Order ID: ");
            return GetPositiveInteger("");
        }

        private void DisplayOrderDetails(Order order)
        {
            Console.WriteLine($"Order ID: {order.Id}");
            Console.WriteLine("Items in this Order:");
            foreach (var item in order.Items)
            {
                Console.WriteLine($"- Product: {item.ProductName}, Quantity: {item.Quantity}, Unit Price: {item.UnitPrice}, Total Price: {item.TotalPrice}");
            }
            Console.WriteLine($"Total Order Amount: {order.TotalAmount}");
        }

        private void ExitApplication()
        {
            Console.WriteLine("Exiting the application.");
            Environment.Exit(0);
        }
    }
}

