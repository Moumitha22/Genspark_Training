namespace ChienVHShopOnline.Models.DTOs
{
    public class NewsRequestDto
    {
        public int? UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public IFormFile? Image { get; set; }
        public string? Content { get; set; }
        public int? Status { get; set; }
    }
}