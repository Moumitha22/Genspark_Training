namespace ChienVHShopOnline.Models.DTOs
{
    public class ContactUsRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Content { get; set; }
        public string CaptchaToken { get; set; } = string.Empty; // reCAPTCHA v2 or v3 token
    }
    
}