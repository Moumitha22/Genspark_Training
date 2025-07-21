using TrainingVideoPortalAPI.Models;
using TrainingVideoPortalAPI.Models.DTO;

namespace TrainingVideoPortalAPI.Interfaces
{
    public interface ITrainingVideoService
    {
        Task<TrainingVideoResponseDto> UploadVideoAsync(TrainingVideoUploadRequestDto dto);
        Task<IEnumerable<TrainingVideoResponseDto>> GetAllVideosAsync();
        Task<TrainingVideoResponseDto?> GetVideoByIdAsync(Guid id);
    }

}