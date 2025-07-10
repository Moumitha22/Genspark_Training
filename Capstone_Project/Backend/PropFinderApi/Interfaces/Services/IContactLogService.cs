using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;
using PropFinderApi.Models.DTOs.Responses;

namespace PropFinderApi.Interfaces
{
    public interface IContactLogService
    {
        Task<ContactListerResponseDto> ContactListerAsync(ContactListerRequestDto requestDto, Guid buyerId);
        Task<IEnumerable<ContactLog>> GetAllContactLogs();
        Task<IEnumerable<ListerInquiryDto>> GetContactLogsForPropertyAsync(Guid propertyId, Guid requesterId, string role);
        Task<IEnumerable<ListerInquiryDto>> GetContactLogsForListerAsync(Guid listerId, Guid requesterId, string role);
        Task<IEnumerable<BuyerInquiryDto>> GetContactLogsForBuyerAsync(Guid buyerId, Guid requesterId, string role);
    }
}
