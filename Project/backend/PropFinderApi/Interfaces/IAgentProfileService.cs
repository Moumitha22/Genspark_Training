using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IAgentProfileService
    {
        Task<IEnumerable<AgentProfile>> GetAllAsync();
        Task<AgentProfile> GetByIdAsync(Guid id);
        Task<AgentProfile?> GetAgentProfileByAgentIdAsync(Guid agentId);
        Task<AgentProfile> CreateAgentProfileAsync(AgentProfileAddRequestDto agentProfileDto, Guid agentId);
        Task<AgentProfile?> UpdateAgentProfileAsync(Guid profileId, AgentProfileAddRequestDto dto, Guid requesterId, string userRole);
    }
}
