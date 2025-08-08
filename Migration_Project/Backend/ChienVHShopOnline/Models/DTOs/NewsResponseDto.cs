namespace ChienVHShopOnline.Models.DTOs
{
    public class NewsResponseDto
    {
         public int Id { get; set; }
        public int? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public string? Image { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? Status { get; set; }
    }
}