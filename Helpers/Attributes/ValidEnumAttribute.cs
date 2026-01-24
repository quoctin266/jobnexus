using System;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class ValidEnumAttribute : ValidationAttribute
    {
        public Type EnumType { get; }

        public ValidEnumAttribute(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException("Type must be an enum.", nameof(enumType));

            EnumType = enumType;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Let [Required] handle null checks
            var str = value as string;

            if (string.IsNullOrWhiteSpace(str)) return ValidationResult.Success;

            if(!int.TryParse(str, out _) && Enum.TryParse(EnumType, str, ignoreCase: true, out var enumValue) 
                && Enum.IsDefined(EnumType, enumValue))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? $"The value '{str}' is not valid for enum {EnumType.Name}.");
        }
    }
}
