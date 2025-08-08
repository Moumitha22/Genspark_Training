using System.Security.Claims;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Interfaces;

public interface IAuthService
{
    Task<UserLoginResponseDto> LoginAsync(UserLoginRequestDto userLoginDto);
    public Task LogoutAsync(string refreshToken);
    Task<User> GetCurrentUserAsync(ClaimsPrincipal user);
}
