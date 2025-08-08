namespace ChienVHShopOnline.Models.DTOs
{
    public class CartItemResponseDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double? UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
