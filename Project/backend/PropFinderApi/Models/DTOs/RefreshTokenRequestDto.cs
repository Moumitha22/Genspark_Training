using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Models.DTOs
{
    public class RefreshTokenRequestDto
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        [MinLength(20, ErrorMessage = "Refresh token must be at least 20 characters long.")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
