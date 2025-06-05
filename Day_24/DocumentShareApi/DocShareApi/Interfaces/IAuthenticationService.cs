using DocShareApi.Models.DTOs;

namespace DocShareApi.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponseDto> Login(LoginRequestDto dto);
        Task<string> Register(RegisterRequestDto dto);
    }
}
