using PropFinderApi.Contexts;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Repositories
{
    public class ListerProfileRepository : Repository<Guid, ListerProfile>, IListerProfileRepository
    {
        public ListerProfileRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<ListerProfile> Get(Guid key)
        {
            var profile = await _propFinderDbContext.ListerProfiles.SingleOrDefaultAsync(a => a.Id == key);
            return profile ?? throw new NotFoundException($"Agent profile with ID {key} not found");
        }

        public override async Task<IEnumerable<ListerProfile>> GetAll()
        {
            return await _propFinderDbContext.ListerProfiles.ToListAsync();
        }

        public async Task<ListerProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _propFinderDbContext.ListerProfiles
                .FirstOrDefaultAsync(ap => ap.UserId == userId);
        }

    }
}
