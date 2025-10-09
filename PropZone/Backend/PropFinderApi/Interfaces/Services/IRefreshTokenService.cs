using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(Guid userId);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAndReplaceAsync(string oldTokenValue, string newTokenValue);
        Task<User?> GetUserByRefreshTokenAsync(string token);
    }
}
