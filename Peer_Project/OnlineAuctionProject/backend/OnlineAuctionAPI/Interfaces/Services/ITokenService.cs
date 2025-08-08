using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface ITokenService
{
    public Task<TokenDto> GenerateTokensAsync(User user);
    public Task<UserLoginResponseDto> RefreshTokenAsync(string refreshToken);
}
