namespace TrainingVideoPortalAPI.Models
{
    public class TrainingVideo
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime UploadDate { get; set; }
        public string BlobUrl { get; set; } = null!;
    }
}
