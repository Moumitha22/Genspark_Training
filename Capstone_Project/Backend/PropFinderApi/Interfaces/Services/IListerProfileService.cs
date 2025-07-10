using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Interfaces
{
    public interface IListerProfileService
    {
        Task<IEnumerable<ListerProfile>> GetAllAsync();
        Task<ListerProfile> GetByIdAsync(Guid id);
        Task<ListerProfile?> GetListerProfileByListerIdAsync(Guid ListerId);
        Task<ListerProfile> CreateListerProfileAsync(ListerProfileAddRequestDto ListerProfileDto, Guid ListerId);
        Task<ListerProfile?> UpdateListerProfileAsync(Guid profileId, ListerProfileAddRequestDto dto, Guid requesterId, string userRole);
        Task<bool> IsProfileCompleteAsync(Guid userId);
    }
}
