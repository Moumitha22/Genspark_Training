using System.ComponentModel.DataAnnotations;
using PropFinderApi.Attributes;
using PropFinderApi.Models.Enums;

namespace PropFinderApi.Models.DTOs
{
    public class UserRegisterRequestDto
    {
        [ValidName(3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;

        [StrongPassword]
        public string Password { get; set; } = string.Empty;

        [OptionalPhoneNumber]
        public string? PhoneNumber { get; set; }


        [Required(ErrorMessage = "Role is required.")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Role must be either 'Agent' or 'Buyer'.")]
        public UserRole Role { get; set; }
    }
}
