namespace TrainingVideoPortalAPI.Models.DTO
{
    public class TrainingVideoResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string BlobUrl { get; set; } = null!;
        public DateTime UploadDate { get; set; }
    }
    
}