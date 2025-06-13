using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Attributes
{
    public class ValidNameAttribute : ValidationAttribute
    {
        private readonly int _minLength;

        public ValidNameAttribute(int minLength = 2)
        {
            _minLength = minLength;
            ErrorMessage = $"Name must be at least {_minLength} characters and contain only letters and spaces.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var name = value as string;

            if (string.IsNullOrWhiteSpace(name))
                return new ValidationResult("Name is required.");

            if (name.Length < _minLength)
                return new ValidationResult(ErrorMessage);

            if (name.Length > 100)
                return new ValidationResult("Name cannot exceed 100 characters.");

            if (!name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
