using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IContactLogRepository : IRepository<Guid, ContactLog>
    {
        Task<IEnumerable<ContactLog>> GetByBuyerIdAsync(Guid buyerId);
        Task<IEnumerable<ContactLog>> GetByAgentIdAsync(Guid agentId);
    }
}
