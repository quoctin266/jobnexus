using JobNexus.Common.Constant.Messages;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public class RegisterDto
    {
            [Required]
            [MaxLength(20, ErrorMessage = ValidationMessages.UsernameMaxLength)]
            public string Username { get; set; } = "";

            [Required]
            [EmailAddress]
            [MaxLength(20, ErrorMessage = ValidationMessages.EmailMaxLength)]
            public string Email { get; set; } = "";

            [Required]
            public string Password { get; set; } = "";
    }
}
