using Microsoft.AspNetCore.Mvc;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace PropFinderApi.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IApiResponseMapper _responseMapper;

        public AuthenticationController(IAuthenticationService authService, IApiResponseMapper responseMapper)
        {
            _authService = authService;
            _responseMapper = responseMapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            var response = _responseMapper.MapToOkResponse("User registered successfully", result);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            var loginResponse = await _authService.LoginAsync(loginRequest);
            var response = _responseMapper.MapToOkResponse("Login successful", loginResponse);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var newTokens = await _authService.RefreshTokenAsync(request.RefreshToken);
            var response = _responseMapper.MapToOkResponse("Token refreshed successfully", newTokens);
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            await _authService.LogoutAsync(request.RefreshToken);
            var response = _responseMapper.MapToOkResponse("Logged out successfully");
            return Ok(response);
        }
    }
}
