using PropFinderApi.Contexts;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Repositories
{
    public class AgentProfileRepository : Repository<Guid, AgentProfile>, IAgentProfileRepository
    {
        public AgentProfileRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<AgentProfile> Get(Guid key)
        {
            var profile = await _propFinderDbContext.AgentProfiles.SingleOrDefaultAsync(a => a.Id == key);
            return profile ?? throw new NotFoundException($"Agent profile with ID {key} not found");
        }

        public override async Task<IEnumerable<AgentProfile>> GetAll()
        {
            return await _propFinderDbContext.AgentProfiles.ToListAsync();
        }

        public async Task<AgentProfile?> GetByUserIdAsync(Guid userId)
        {
            return await _propFinderDbContext.AgentProfiles
                .FirstOrDefaultAsync(ap => ap.UserId == userId);
        }

    }
}
