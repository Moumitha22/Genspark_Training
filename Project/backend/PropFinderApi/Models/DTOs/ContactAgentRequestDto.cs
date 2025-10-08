using System.ComponentModel.DataAnnotations;
using PropFinderApi.Attributes;

namespace PropFinderApi.Models.DTOs
{
    public class ContactAgentRequestDto
    {
        [Required(ErrorMessage = "Property ID is required.")]
        public Guid PropertyId { get; set; }

        [ValidPhoneNumber(ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string BuyerPhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string BuyerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
        public string Message { get; set; } = string.Empty;
    }
}
