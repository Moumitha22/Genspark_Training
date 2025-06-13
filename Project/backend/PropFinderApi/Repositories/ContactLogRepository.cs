using PropFinderApi.Contexts;
using PropFinderApi.Models;
using Microsoft.EntityFrameworkCore;
using PropFinderApi.Exceptions;
using PropFinderApi.Interfaces;

namespace PropFinderApi.Repositories
{
    public class ContactLogRepository : Repository<Guid, ContactLog>, IContactLogRepository
    {
        public ContactLogRepository(PropFinderDbContext context) : base(context) { }

        public override async Task<ContactLog> Get(Guid key)
        {
            var contact = await _propFinderDbContext.ContactLogs.SingleOrDefaultAsync(c => c.Id == key);
            return contact ?? throw new NotFoundException($"No contact log found with ID {key}");
        }

        public override async Task<IEnumerable<ContactLog>> GetAll()
        {
            return await _propFinderDbContext.ContactLogs.ToListAsync();
        }

        public async Task<IEnumerable<ContactLog>> GetByBuyerIdAsync(Guid buyerId)
        {
            return await _propFinderDbContext.ContactLogs
                .Where(cl => cl.BuyerId == buyerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ContactLog>> GetByAgentIdAsync(Guid agentId)
        {
            return await _propFinderDbContext.ContactLogs
                .Where(cl => cl.AgentId == agentId)
                .ToListAsync();
        }

    }
}
