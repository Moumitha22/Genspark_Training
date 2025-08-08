using Microsoft.EntityFrameworkCore;
using ChienVHShopOnline.Data;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Interfaces;

namespace ChienVHShopOnline.Repositories
{
    public class RefreshTokenRepository : Repository<int, RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context) { }

        public override async Task<RefreshToken> Get(int key)
        {
            var token = await _dbContext.RefreshTokens.SingleOrDefaultAsync(t => t.Id == key);
            return token ?? throw new KeyNotFoundException($"Refresh token with ID {key} not found");
        }

        public override async Task<IEnumerable<RefreshToken>> GetAll()
        {
            return await _dbContext.RefreshTokens.ToListAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow);
        }

    }
}
