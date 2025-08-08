using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineAuctionAPI.Helpers;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;
using OnlineAuctionAPI.Service;

namespace OnlineAuctionAPI.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authenticationService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthenticationController> _logger;

    public AuthenticationController(
        IAuthService authenticationService,
        ITokenService tokenService,
        ILogger<AuthenticationController> logger
    )
    {
        _authenticationService = authenticationService;
        _tokenService = tokenService;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResponseDto>> UserLogin(
        UserLoginRequestDto loginRequest
    )
    {
        var result = await _authenticationService.LoginAsync(loginRequest);
        _logger.LogInformation("User {Email} logged in successfully", loginRequest.Email);
        return Ok(ApiResponseHelper.CreateSuccess(result, "Login successful"));
    }

    [HttpPost("logout")]
    [Authorize(Roles = "Admin,Seller,Bidder")]
    public async Task<ActionResult> Logout([FromBody] string refreshToken)
    {
        await _authenticationService.LogoutAsync(refreshToken);
        _logger.LogInformation("Logout successful for refresh token {RefreshToken}", refreshToken);
        return Ok(ApiResponseHelper.CreateSuccess<string>(null, "Logout successful"));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<UserLoginResponseDto>> RefreshToken(
        [FromBody] string refreshToken
    )
    {
        var result = await _tokenService.RefreshTokenAsync(refreshToken);
        if (result == null)
        {
            _logger.LogWarning("Invalid refresh token attempted: {RefreshToken}", refreshToken);
            return Unauthorized(
                ApiResponseHelper.CreateUnauthorized<string>("Invalid refresh token")
            );
        }
        _logger.LogInformation(
            "Token refreshed successfully for refresh token {RefreshToken}",
            refreshToken
        );
        return Ok(ApiResponseHelper.CreateSuccess(result, "Token refreshed successfully"));
    }

    [HttpGet("me")]
    [Authorize(Roles = "Admin,Seller,Bidder")]
    public async Task<ActionResult<User>> GetMe()
    {
        var user = await _authenticationService.GetCurrentUserAsync(User);
        return Ok(ApiResponseHelper.CreateSuccess(user, "User profile retrieved successfully"));
    }
}
