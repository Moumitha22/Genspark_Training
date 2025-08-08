using OnlineAuctionAPI.Interfaces;
using OnlineAuctionAPI.Models;

namespace OnlineAuctionAPI.Service;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using OnlineAuctionAPI.Contexts;
using OnlineAuctionAPI.Models.DTO;

public class TokenService : ITokenService
{
    private readonly SymmetricSecurityKey _securityKey;
    private readonly int _accessTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;
    private readonly IUserRepository _userRepository;

    public TokenService(IConfiguration configuration, IUserRepository userRepository)
    {
        var jwtTokenKey =
            configuration["Keys:JwtTokenKey"]
            ?? throw new ArgumentNullException("JwtTokenKey configuration is missing.");
        _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenKey));
        _accessTokenExpiryMinutes = configuration.GetValue<int>(
            "JwtSettings:AccessTokenExpiryMinutes"
        );
        _refreshTokenExpiryDays = configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryDays");
        _userRepository = userRepository;
    }

    public async Task<TokenDto> GenerateTokensAsync(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("UserId", user.Id.ToString()),
        };

        var creds = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            SigningCredentials = creds,
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = tokenHandler.WriteToken(token);

        var refreshToken = GenerateRefreshToken();
        var newRefreshToken = new RefreshToken
        {
            Token = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays),
            IsRevoked = false,
            UserId = user.Id,
        };
        await _userRepository.AddRefreshTokenAsync(newRefreshToken);
        return new TokenDto { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var randomNumGen = RandomNumberGenerator.Create();
        randomNumGen.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<UserLoginResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await _userRepository.GetRefreshTokenAsync(refreshToken);
        if (
            storedRefreshToken == null
            || storedRefreshToken.ExpiresAt < DateTime.UtcNow
            || storedRefreshToken.IsRevoked
        )
        {
            throw new UnauthorizedAccessException("Invalid, expired, or revoked refresh token");
        }

        var user = await _userRepository.Get(storedRefreshToken.UserId);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found for refresh token");
        }

        storedRefreshToken.IsRevoked = true;
        storedRefreshToken.RevokedAt = DateTime.UtcNow;

        var newTokens = await GenerateTokensAsync(user);

        return new UserLoginResponseDto
        {
            UserName = user.Username,
            Email = user.Email,
            Token = newTokens.AccessToken,
            RefreshToken = newTokens.RefreshToken,
        };
    }
}
