namespace ChienVHShopOnline.Models.DTOs
{
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public string OrderName { get; set; } = string.Empty;
        public DateTime? OrderDate { get; set; }
        public string PaymentType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;

        public List<OrderDetailsResponseDto> OrderDetails { get; set; } = new();
    }
}
