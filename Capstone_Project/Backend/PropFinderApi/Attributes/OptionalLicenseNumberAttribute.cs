using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Attributes
{
    public class OptionalLicenseNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var license = (value as string)?.Trim();

            // Optional
            if (string.IsNullOrWhiteSpace(license))
                return ValidationResult.Success;

            var parts = license.Split('/');
            if (parts.Length != 4)
                return new ValidationResult("License format should be like 'TS/AGENT/23/1234'.");

            if (parts[0].Length != 2 || !parts[0].All(char.IsUpper))
                return new ValidationResult("License must start with a 2-letter state code (e.g., 'TS').");

            if (parts[1] != "AGENT" && parts[1] != "AGENCY")
                return new ValidationResult("License must contain 'AGENT' or 'AGENCY'.");

            if (parts[2].Length != 2 || !parts[2].All(char.IsDigit))
                return new ValidationResult("Year part of license must be 2 digits (e.g., '23').");

            if (parts[3].Length < 3 || parts[3].Length > 5 || !parts[3].All(char.IsDigit))
                return new ValidationResult("Serial number must be 3 to 5 digits.");

            return ValidationResult.Success;
        }
    }
}
