namespace OrderProcessingApplication.Models
{
    public class OrderItem
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }

        // Calculates total price for this item
        public virtual double TotalPrice => Quantity * UnitPrice;
        // making TotalPrice virtual so derived class can override it
        // Eg. DiscountedOrderItem

    }
}
