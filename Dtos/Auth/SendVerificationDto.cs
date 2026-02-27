using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public record SendVerificationDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(20, ErrorMessage = ValidationMessages.EmailMaxLength)]
        public string Email { get; init; } = "";

        [Required]
        public TokenPurpose Purpose { get; init; }
    }
}
