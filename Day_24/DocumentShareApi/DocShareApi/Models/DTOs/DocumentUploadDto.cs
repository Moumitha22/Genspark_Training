using System.ComponentModel.DataAnnotations;

namespace DocShareApi.Models.DTOs
{
    public class DocumentUploadDto
    {
        [Required]
        public IFormFile File { get; set; }

        public string? Description { get; set; }
    }
}
