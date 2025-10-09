using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Models.DTOs
{
    public class PropertyLocationAddRequestDto
    {
        [Required(ErrorMessage = "Locality is required.")]
        [StringLength(150)]
        public string Locality { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
