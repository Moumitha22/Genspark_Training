using Microsoft.AspNetCore.Http;
using TrainingVideoPortalAPI.Interfaces;
using TrainingVideoPortalAPI.Models;
using TrainingVideoPortalAPI.Models.DTO;

namespace TrainingVideoPortalAPI.Services
{
    public class TrainingVideoService : ITrainingVideoService
    {
        private readonly IRepository<Guid, TrainingVideo> _repository;
        private readonly BlobStorageService _blobStorageService;

        public TrainingVideoService(
            IRepository<Guid, TrainingVideo> repository,
            BlobStorageService blobStorageService)
        {
            _repository = repository;
            _blobStorageService = blobStorageService;
        }

        public async Task<TrainingVideoResponseDto> UploadVideoAsync(TrainingVideoUploadRequestDto dto)
        {
            var blobUrl = await _blobStorageService.UploadFileAsync(dto.File);
            var video = new TrainingVideo
            {
                Title = dto.Title,
                Description = dto.Description,
                UploadDate = DateTime.UtcNow,
                BlobUrl = blobUrl
            };

            var savedVideo = await _repository.Add(video);
            return MapToDto(savedVideo);
        }

        public async Task<IEnumerable<TrainingVideoResponseDto>> GetAllVideosAsync()
        {
            var videos = await _repository.GetAll();
            return videos.Select(MapToDto);
        }

        public async Task<TrainingVideoResponseDto?> GetVideoByIdAsync(Guid id)
        {
            try
            {
                var video = await _repository.Get(id);
                return MapToDto(video);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }
        
        private TrainingVideoResponseDto MapToDto(TrainingVideo video)
        {
            return new TrainingVideoResponseDto
            {
                Id = video.Id,
                Title = video.Title,
                Description = video.Description,
                UploadDate = video.UploadDate,
                BlobUrl = video.BlobUrl
            };
        }
    }
}
