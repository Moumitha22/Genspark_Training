namespace OnlineAuctionAPI.Models.DTO;

public class UserLoginResponseDto
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration{ get; set; }
}