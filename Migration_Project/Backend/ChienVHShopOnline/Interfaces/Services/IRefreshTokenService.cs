using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshTokenAsync(int userId);
        Task<bool> IsRefreshTokenValidAsync(string refreshToken);
        Task RevokeRefreshTokenAsync(string refreshToken);
        Task RevokeAndReplaceAsync(string oldTokenValue, string newTokenValue);
        Task<User?> GetUserByRefreshTokenAsync(string token);
    }
}
