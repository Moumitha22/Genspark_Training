using System.ComponentModel.DataAnnotations;

namespace ChienVHShopOnline.Models.DTOs
{
    public class UserRegisterRequestDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
