using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
     public interface IAuthenticationService
    {
        Task<string> RegisterAsync(UserRegisterRequestDto registerRequest);
        Task<UserLoginResponseDto> LoginAsync(UserLoginRequestDto loginRequest);
        Task LogoutAsync(string userId);
        Task<UserLoginResponseDto> RefreshTokenAsync(string refreshToken);
    }
}