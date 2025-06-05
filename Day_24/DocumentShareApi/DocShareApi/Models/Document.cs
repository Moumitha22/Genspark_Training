using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocShareApi.Models { 

    public class Document
    {
        [Key]
        public Guid DocumentId { get; set; } = Guid.NewGuid();

        public string FileName { get; set; }  = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public string Description { get; set; }   = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public string UploadedBy { get; set; }  = string.Empty;

        // [ForeignKey("UploadedBy")]
        public User Uploader { get; set; }

        public string Status { get; set; } = string.Empty;

    }
}
