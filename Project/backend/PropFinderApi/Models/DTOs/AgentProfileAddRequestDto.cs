using System.ComponentModel.DataAnnotations;
using PropFinderApi.Attributes;

namespace PropFinderApi.Models.DTOs
{
    public class AgentProfileAddRequestDto
    {
        [OptionalLicenseNumber(ErrorMessage =" License format should be like 'TS/AGENT/23/1234'.")]
        public string? LicenseNumber { get; set; }

        [StringLength(100, ErrorMessage = "Agency name cannot exceed 100 characters.")]
        public string? AgencyName { get; set; }

        [ValidPhoneNumber(ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string BusinessPhoneNumber { get; set; } = string.Empty;
    }
}
