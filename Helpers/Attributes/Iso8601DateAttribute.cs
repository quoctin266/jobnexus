using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace JobNexus.Helpers.Attributes
{
    public sealed class Iso8601DateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str))
            {
                // Let [Required] handle empty/null values
                return ValidationResult.Success;
            }

            // Try parse as full ISO 8601 date-time with offset / Z using DateTimeOffset
            if (DateTimeOffset.TryParseExact(str, @"yyyy-MM-dd\THH:mm:ss.fff\Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "The field must be a valid ISO 8601 date-time string.");
        }
    }
}
