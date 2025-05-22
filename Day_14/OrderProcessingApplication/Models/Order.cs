using System.Collections.Generic;

namespace OrderProcessingApplication.Models
{
    public class Order
    {
        public int Id { get; set; }

        public List<OrderItem> Items { get; set; } = new();

        // Computes the total order amount from all items
        public double TotalAmount => CalculateTotal();

        private double CalculateTotal()
        {
            double sum = 0;
            foreach (var item in Items)
            {
                sum += item.TotalPrice;
            }
            return sum;
        }
    }
}
