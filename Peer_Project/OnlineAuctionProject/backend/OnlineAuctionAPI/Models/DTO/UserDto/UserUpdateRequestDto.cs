using OnlineAuctionAPI.Validation;
namespace OnlineAuctionAPI.Models.DTO;

public class UserUpdateRequestDto
{
    [NameValidation]
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    [RoleValidation]
    public string Role { get; set; } = string.Empty;
}