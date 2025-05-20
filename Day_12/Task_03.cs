using System;
using System.Collections.Generic;

namespace Day_12
{
    public class Task_03
    {
        public static void Run()
        {
            Dictionary<string, double> products = new Dictionary<string, double>();

            // Add 5 products
            products.Add("Laptop", 75000.00);
            products.Add("Smartphone", 30000.00);
            products.Add("Headphones", 1500.50);
            products.Add("Keyboard", 1200.00);
            products.Add("Monitor", 18000.75);

            ConsoleHelper.PrintGreen("\nAll Products and Prices:\n");

            foreach (var item in products)
            {
                ConsoleHelper.PrintBlue($"{item.Key}: {item.Value:F2}");
            }

            // Search for a specific product
            string searchProduct = UserInputHelper.GetValidString("\nEnter the product name to search: ");

            if (products.ContainsKey(searchProduct))
            {
                double price = products[searchProduct];
                ConsoleHelper.PrintBlue($"The price of '{searchProduct}' is ₹{price:F2}\n");
            }
            else
            {
                ConsoleHelper.PrintRed($"Product '{searchProduct}' not found.\n");
            }
        }
    }
}
