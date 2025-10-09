using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class ContactLogMapper
    {
        public ContactLog MapContactAgentRequestDtoToContactLog(ContactListerRequestDto dto, Guid buyerId, Guid listerId)
        {
            return new ContactLog
            {
                Id = Guid.NewGuid(),
                PropertyId = dto.PropertyId,
                BuyerId = buyerId,
                ListerId = listerId,
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
