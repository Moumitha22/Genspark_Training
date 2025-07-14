using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Attributes
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        private readonly int _minLength = 8;
        private readonly string _specialCharacters = "!@#$%^&*()-_=+[]{};:,.<>?";

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string password || string.IsNullOrWhiteSpace(password))
                return new ValidationResult("Password is required.");

            if (password.Length < _minLength)
                return new ValidationResult($"Password must be at least {_minLength} characters long.");

            if (!password.Any(char.IsUpper))
                return new ValidationResult("Password must contain at least one uppercase letter.");

            if (!password.Any(char.IsLower))
                return new ValidationResult("Password must contain at least one lowercase letter.");

            if (!password.Any(char.IsDigit))
                return new ValidationResult("Password must contain at least one number.");

            if (!password.Any(c => _specialCharacters.Contains(c)))
                return new ValidationResult($"Password must contain at least one special character ({_specialCharacters}).");

            return ValidationResult.Success;
        }
    }

}
