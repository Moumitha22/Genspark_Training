using PropFinderApi.Models;

namespace PropFinderApi.Interfaces
{
    public interface IAgentProfileRepository : IRepository<Guid, AgentProfile>
    {
        Task<AgentProfile?> GetByUserIdAsync(Guid userId);
    }
}
