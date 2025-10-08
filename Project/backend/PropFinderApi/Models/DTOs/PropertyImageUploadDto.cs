using System.ComponentModel.DataAnnotations;
using PropFinderApi.Attributes;

namespace PropFinderApi.Models.DTOs
{
    public class PropertyImageUploadDto
    {
        [Required(ErrorMessage = "Property ID is required.")]
        public Guid PropertyId { get; set; }

        [Required(ErrorMessage = "Image file is required.")]
        [FileValidation(5, "jpg,jpeg,png,webp")]
        public IFormFile File { get; set; } = null!;
    }
}

