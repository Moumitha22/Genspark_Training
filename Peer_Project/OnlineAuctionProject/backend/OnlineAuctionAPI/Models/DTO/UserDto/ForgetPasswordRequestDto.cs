namespace OnlineAuctionAPI.Models.DTO;

public class ForgetPasswordRequestDto
{
    public required string Email { get; set; }
    public required string NewPassword { get; set; }
}