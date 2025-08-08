using ChienVHShopOnline.Models;

namespace ChienVHShopOnline.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<int, RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
