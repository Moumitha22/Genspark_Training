using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Attributes
{
    public class FileValidationAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes;
        private readonly string[] _allowedExtensions;

        public FileValidationAttribute(int maxFileSizeMB, string allowedExtensions)
        {
            _maxFileSizeInBytes = maxFileSizeMB * 1024 * 1024;
            _allowedExtensions = allowedExtensions.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSizeInBytes)
                    return new ValidationResult($"Maximum allowed file size is {_maxFileSizeInBytes / (1024 * 1024)}MB.");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension.TrimStart('.')))
                    return new ValidationResult($"Allowed file extensions are: {string.Join(", ", _allowedExtensions)}.");
            }

            return ValidationResult.Success;
        }
    }
}
