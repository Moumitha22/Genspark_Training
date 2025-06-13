using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class ContactLogMapper
    {
        public ContactLog MapContactAgentRequestDtoToContactLog(ContactAgentRequestDto dto, Guid buyerId, Guid agentId)
        {
            return new ContactLog
            {
                Id = Guid.NewGuid(),
                PropertyId = dto.PropertyId,
                BuyerId = buyerId,
                AgentId = agentId,
                BuyerPhoneNumber = dto.BuyerPhoneNumber,
                BuyerEmail = dto.BuyerEmail.Trim().ToLower(),
                Message = SanitizeMessage(dto.Message),
                CreatedAt = DateTime.UtcNow
            };
        }

        private string SanitizeMessage(string message)
        {
            return string.Join(" ", message.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

    }
}
