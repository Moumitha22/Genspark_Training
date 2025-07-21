using Microsoft.AspNetCore.Mvc;
using TrainingVideoPortalAPI.Interfaces;
using TrainingVideoPortalAPI.Models;
using TrainingVideoPortalAPI.Models.DTO;

namespace TrainingVideoPortalAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainingVideosController : ControllerBase
    {
        private readonly ITrainingVideoService _videoService;

        public TrainingVideosController(ITrainingVideoService videoService)
        {
            _videoService = videoService;
        }

        [Consumes("multipart/form-data")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo([FromForm] TrainingVideoUploadRequestDto dto)
        {
            if (dto.File == null || dto.File.Length == 0)
                return BadRequest("Video file is required.");

            var video = await _videoService.UploadVideoAsync(dto);
            return CreatedAtAction(nameof(GetVideoById), new { id = video.Id }, video);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainingVideo>>> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return Ok(videos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TrainingVideo>> GetVideoById(Guid id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null)
                return NotFound();

            return Ok(video);
        }
    }
}
