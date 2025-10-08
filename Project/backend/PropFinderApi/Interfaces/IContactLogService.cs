using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IContactLogService
    {
        Task<ContactAgentResponseDto> ContactAgentAsync(ContactAgentRequestDto requestDto, Guid buyerId);
        Task<IEnumerable<ContactLog>> GetAllContactLogs();
        Task<IEnumerable<ContactLog>> GetContactLogsForAgentAsync(Guid agentId, Guid requesterId, string role);
        Task<IEnumerable<ContactLog>> GetContactLogsForBuyerAsync(Guid buyerId, Guid requesterId, string role);
    }
}
