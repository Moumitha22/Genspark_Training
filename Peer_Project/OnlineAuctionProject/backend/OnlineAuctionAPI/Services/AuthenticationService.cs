using System.Security.Claims;
using OnlineAuctionAPI.Exceptions;
using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;
using OnlineAuctionAPI.Models.DTO;

namespace OnlineAuctionAPI.Service;

public class AuthenticationService : IAuthService
{
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;

    public AuthenticationService(
        ITokenService tokenService,
        IPasswordService passwordService,
        IUserRepository userRepository
    )
    {
        _tokenService = tokenService;
        _passwordService = passwordService;
        _userRepository = userRepository;
    }

    public async Task<UserLoginResponseDto> LoginAsync(UserLoginRequestDto userLoginDto)
    {
        var dbUser = await _userRepository.GetByEmailAsync(userLoginDto.Email);
        if (dbUser == null)
        {
            throw new NotFoundException("Email not found, please verify the email");
        }
        if (dbUser.StatusId == 2)
        {
            throw new InvalidException("User has been disabled");
        }
        var passwordVerify = _passwordService.VerifyPassword(
            dbUser.Password,
            userLoginDto.Password
        );
        if (!passwordVerify)
        {
            throw new InvalidException("Invalid Password");
        }

        var tokens = await _tokenService.GenerateTokensAsync(dbUser);
        return new UserLoginResponseDto
        {
            UserName = dbUser.Username,
            Email = dbUser.Email,
            Token = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken,
        };
    }

    public async Task LogoutAsync(string refreshToken)
    {
        var user = await _userRepository.GetUserByRefreshTokenAsync(refreshToken);
        if (user == null)
        {
            throw new InvalidException("Invalid refresh token");
        }
        var token = user.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken && !t.IsRevoked);
        if (token == null)
            throw new InvalidException("Refresh token not found");

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;

        await _userRepository.Update(user.Id, user);
    }

    public async Task<User> GetCurrentUserAsync(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId");
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            throw new UnauthorizedAccessException("User not authenticated.");

        var dbUser = await _userRepository.Get(userId);
        if (dbUser == null)
            throw new NotFoundException("User not found.");

        return dbUser;
    }
}
