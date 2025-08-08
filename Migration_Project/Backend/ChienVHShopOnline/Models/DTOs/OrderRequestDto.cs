namespace ChienVHShopOnline.Models.DTOs
{
    public class OrderRequestDto
    {
        public string OrderName { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;

        public List<OrderDetailRequestDto> OrderDetails { get; set; } = new();
    }

}