using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Attributes
{
    public class OptionalPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            
            var phone = (value as string)?.Trim();

            if (string.IsNullOrWhiteSpace(phone))
                return ValidationResult.Success; 

            if (phone.Length != 10 || !phone.All(char.IsDigit))
                return new ValidationResult("Phone number must be exactly 10 digits.");

            if (!"6789".Contains(phone[0]))
                return new ValidationResult("Phone number must start with 6, 7, 8, or 9.");

            return ValidationResult.Success;
        }
    }
}
