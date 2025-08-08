namespace ChienVHShopOnline.Models.DTOs
{
    public class OrderDetailsResponseDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        
        public string? ProductName { get; set; }
    }
    
}