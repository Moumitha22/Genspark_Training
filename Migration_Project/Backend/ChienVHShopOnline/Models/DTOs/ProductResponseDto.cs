namespace ChienVHShopOnline.Models.DTOs
{
    public class ProductResponseDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Image { get; set; }
        public double? Price { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? ColorId { get; set; }
        public int? ModelId { get; set; }
        public int? StorageId { get; set; }

        public string? SellStartDate { get; set; }
        public string? SellEndDate { get; set; }

        public string? CategoryName { get; set; }
        public string? ColorName { get; set; }
        public string? ModelName { get; set; }
        public string? StorageName { get; set; }
        public bool IsNew { get; set; }

    }

}
