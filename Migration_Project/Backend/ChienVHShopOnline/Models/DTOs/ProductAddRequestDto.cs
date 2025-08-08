namespace ChienVHShopOnline.Models.DTOs
{
    public class ProductAddDto
    {
        public string ProductName { get; set; } = string.Empty;
        public IFormFile? Image { get; set; } 
        public double? Price { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }
        public int? StorageId { get; set; }
        public DateTime? SellStartDate { get; set; }
        public DateTime? SellEndDate { get; set; }
        public int? IsNew { get; set; }
    }

}
