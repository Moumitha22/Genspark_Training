using Microsoft.AspNetCore.Mvc;
using ChienVHShopOnline.Models.DTOs;
using ChienVHShopOnline.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace ChienVHShopOnline.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly IWebHostEnvironment _env;

        public AuthenticationController(
            IAuthenticationService authService,
            IWebHostEnvironment env)
        {
            _authService = authService;
            _env = env;
        }

        private CookieOptions GetRefreshTokenCookieOptions()
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterRequestDto registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            return Ok(new { message = "User registered successfully", data = result });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequestDto loginRequest)
        {
            var loginResponse = await _authService.LoginAsync(loginRequest);

            // Set the refresh token in HttpOnly cookie
            Response.Cookies.Append("refreshToken", loginResponse.RefreshToken, GetRefreshTokenCookieOptions());

            // Hide the refresh token from body response
            loginResponse.RefreshToken = null;

            return Ok(new { message = "Login successful", data = loginResponse });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return BadRequest(new { message = "Missing refresh token" });

            var newTokens = await _authService.RefreshTokenAsync(refreshToken);

            // Replace the old cookie with the new token
            Response.Cookies.Append("refreshToken", newTokens.RefreshToken, GetRefreshTokenCookieOptions());

            newTokens.RefreshToken = null;

            return Ok(new { message = "Token refreshed successfully", data = newTokens });
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

            // Clear the cookie
            Response.Cookies.Append("refreshToken", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = SameSiteMode.Strict,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddDays(-1)
            });

            return Ok(new { message = "Logged out successfully" });
        }
    }
}
