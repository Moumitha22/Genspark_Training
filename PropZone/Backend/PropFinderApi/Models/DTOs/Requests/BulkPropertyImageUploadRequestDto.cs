using System.ComponentModel.DataAnnotations;
using PropFinderApi.Attributes;

namespace PropFinderApi.Models.DTOs
{
    public class BulkPropertyImageUploadRequestDto
    {
        [Required(ErrorMessage = "Property ID is required.")]
        public Guid PropertyId { get; set; }

        [FileListValidation(1, "jpg,jpeg,png,webp")]
        public List<IFormFile> Files { get; set; } = new();
    }
}