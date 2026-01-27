using System.ComponentModel.DataAnnotations;

namespace JobNexus.Helpers.Attributes
{
    public class ValidExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public ValidExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ext) || !_extensions.Contains(ext))
                    return new ValidationResult("Invalid file type");
            }

            return ValidationResult.Success;
        }
    }
}
