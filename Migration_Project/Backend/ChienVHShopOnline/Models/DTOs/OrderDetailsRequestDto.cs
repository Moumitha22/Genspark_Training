namespace ChienVHShopOnline.Models.DTOs
{
    public class OrderDetailRequestDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
