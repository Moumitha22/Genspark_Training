using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<Guid, RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
    }
}
