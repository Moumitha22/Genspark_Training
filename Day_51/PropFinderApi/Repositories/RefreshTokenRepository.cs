using Microsoft.EntityFrameworkCore;
using PropFinderApi.Contexts;
using PropFinderApi.Models;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Repositories
{
    public class RefreshTokenRepository : Repository<Guid, RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<RefreshToken> Get(Guid key)
        {
            var token = await _propFinderDbContext.RefreshTokens.SingleOrDefaultAsync(t => t.Id == key);
            return token ?? throw new NotFoundException($"Refresh token with ID {key} not found");
        }

        public override async Task<IEnumerable<RefreshToken>> GetAll()
        {
            return await _propFinderDbContext.RefreshTokens.ToListAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _propFinderDbContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow);
        }

    }
}
