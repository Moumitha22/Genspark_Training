namespace OnlineAuctionAPI.Models.DTO;

using OnlineAuctionAPI.Validation;

public class ChangePasswordRequestDto
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; }
    [PasswordValidation]
    public string NewPassword { get; set; }
}