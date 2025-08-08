using System.ComponentModel.DataAnnotations;
using OnlineAuctionAPI.Validation;

namespace OnlineAuctionAPI.Models.DTO;

public class UserRegisterRequestDto
{
    [Required]
    [NameValidation]
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    [Required]
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RoleValidation]
    public string Role { get; set; } = string.Empty;
}
