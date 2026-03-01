using JobNexus.Common.Constant.Messages;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public record ResetPasswordDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(50, ErrorMessage = ValidationMessages.EmailMaxLength)]
        public string Email { get; init; } = "";

        [Required]
        public string NewPassword { get; init; } = "";

        [Required]
        public string Token { get; init; } = "";
    }
}
