using System.Security.Cryptography;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;
using PropFinderApi.Models;

namespace PropFinderApi.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
        }

        public async Task<string> GenerateRefreshTokenAsync(Guid userId)
        {
            var tokenBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(tokenBytes);
            var tokenString = Convert.ToBase64String(tokenBytes);

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = tokenString,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // expires in 7 days
            };

            await _refreshTokenRepository.Add(refreshToken);
            return tokenString;
        }

        public async Task<bool> IsRefreshTokenValidAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            return refreshToken != null && refreshToken.RevokedAt == null && refreshToken.ExpiresAt > DateTime.UtcNow;
        }

        public async Task RevokeRefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            if (refreshToken == null)
                throw new NotFoundException("Refresh token not found");

            refreshToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.Update(refreshToken.Id, refreshToken);
        }

        public async Task RevokeAndReplaceAsync(string oldTokenValue, string newTokenValue)
        {
            var oldToken = await _refreshTokenRepository.GetByTokenAsync(oldTokenValue);
            if (oldToken == null)
                throw new NotFoundException("Old refresh token not found");

            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReplacedByToken = newTokenValue;

            await _refreshTokenRepository.Update(oldToken.Id, oldToken);
        }


        public async Task<User?> GetUserByRefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
            if (refreshToken == null)
                return null;

            var user = await _userRepository.Get(refreshToken.UserId);
            return user;
        }
    }
}
