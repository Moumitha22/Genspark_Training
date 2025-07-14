using System.ComponentModel.DataAnnotations;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class UserLoginRequestDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Lister' or 'Buyer'.")]
        public UserRole? Role { get; set; }
    }
    
}