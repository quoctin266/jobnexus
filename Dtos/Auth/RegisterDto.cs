using JobNexus.Common.Constant.Messages;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public record RegisterDto
    {
            [Required]
            [MaxLength(50, ErrorMessage = ValidationMessages.UsernameMaxLength)]
            public string Username { get; init; } = "";

            [Required]
            [EmailAddress]
            [MaxLength(50, ErrorMessage = ValidationMessages.EmailMaxLength)]
            public string Email { get; init; } = "";

            [Required]
            public string Password { get; init; } = "";
    }
}
