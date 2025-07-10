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
        private readonly IWebHostEnvironment _env;

        public AuthenticationController(
            IAuthenticationService authService,
            IApiResponseMapper responseMapper,
            IWebHostEnvironment env)
        {
            _authService = authService;
            _responseMapper = responseMapper;
            _env = env;
        }

        private CookieOptions GetRefreshTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(), // only Secure=true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
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

            // Set the refresh token in HttpOnly cookie
            Response.Cookies.Append("refreshToken", loginResponse.RefreshToken, GetRefreshTokenCookieOptions());

            // Remove refresh token from body before returning
            loginResponse.RefreshToken = null;

            var response = _responseMapper.MapToOkResponse("Login successful", loginResponse);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest("Missing refresh token");

            var newTokens = await _authService.RefreshTokenAsync(refreshToken);

            // Replace the old cookie with new refresh token
            Response.Cookies.Append("refreshToken", newTokens.RefreshToken, GetRefreshTokenCookieOptions());

            newTokens.RefreshToken = null;

            var response = _responseMapper.MapToOkResponse("Token refreshed successfully", newTokens);
            return Ok(response);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken);
            }

            Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            });

            var response = _responseMapper.MapToOkResponse("Logged out successfully");
            return Ok(response);
        }
    }
}
