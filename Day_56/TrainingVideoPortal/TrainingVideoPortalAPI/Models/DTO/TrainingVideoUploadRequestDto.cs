namespace TrainingVideoPortalAPI.Models.DTO
{
    public class TrainingVideoUploadRequestDto
    {
        public IFormFile File { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
