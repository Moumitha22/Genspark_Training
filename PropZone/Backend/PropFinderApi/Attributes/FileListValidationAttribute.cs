using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace PropFinderApi.Attributes
{
    public class FileListValidationAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInMB;
        private readonly string[] _allowedExtensions;

        public FileListValidationAttribute(int maxSizeInMB, string allowedExtensions)
        {
            _maxSizeInMB = maxSizeInMB;
            _allowedExtensions = allowedExtensions.Split(',');
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var files = value as List<IFormFile>;
            if (files == null || files.Count == 0)
                return new ValidationResult("At least one file is required.");

            foreach (var file in files)
            {
                if (file.Length > _maxSizeInMB * 1024 * 1024)
                    return new ValidationResult($"Each file must be less than {_maxSizeInMB}MB.");

                var ext = Path.GetExtension(file.FileName).TrimStart('.').ToLowerInvariant();
                if (!_allowedExtensions.Contains(ext))
                    return new ValidationResult($"Only the following extensions are allowed: {string.Join(", ", _allowedExtensions)}.");
            }

            return ValidationResult.Success;
        }
    }
}
