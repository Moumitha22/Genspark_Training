using PropFinderApi.Models;
using PropFinderApi.Models.DTOs;

namespace PropFinderApi.Mappers
{
    public class AgentProfileMapper
    {
        public AgentProfile MapAgentProfileRequestDtoToAgentProfile(AgentProfileAddRequestDto dto, Guid userId)
        {
            var sanitizedLicenseNumber = SanitizeLicenseNumber(dto.LicenseNumber);
            return new AgentProfile
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LicenseNumber =  sanitizedLicenseNumber,
                AgencyName = SanitizeAgencyName(dto.AgencyName),
                BusinessPhoneNumber = dto.BusinessPhoneNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsVerifiedAgent = !string.IsNullOrEmpty(sanitizedLicenseNumber),
                IsDeleted = false
            };
        }

        public AgentProfile MapUpdatedAgentProfile(AgentProfile existing, AgentProfileAddRequestDto dto)
        {
            if (!string.IsNullOrWhiteSpace(dto.LicenseNumber))
            {
                var sanitizedLicenseNumber = SanitizeLicenseNumber(dto.LicenseNumber);
                existing.LicenseNumber = sanitizedLicenseNumber;
                existing.IsVerifiedAgent = !string.IsNullOrWhiteSpace(sanitizedLicenseNumber);
            }

            if (!string.IsNullOrWhiteSpace(dto.AgencyName))
            {
                existing.AgencyName = SanitizeAgencyName(dto.AgencyName);
            }

            if (!string.IsNullOrWhiteSpace(dto.BusinessPhoneNumber))
            {
                existing.BusinessPhoneNumber = dto.BusinessPhoneNumber.Trim();
            }

            existing.UpdatedAt = DateTime.UtcNow;

            return existing;
        }

        private string SanitizeLicenseNumber(string? license)
        {
            return string.IsNullOrWhiteSpace(license) ? "" : license.Trim();
        }

        private string SanitizeAgencyName(string? agencyName)
        {
            if (string.IsNullOrWhiteSpace(agencyName)) return "";

            return string.Join(" ", agencyName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
