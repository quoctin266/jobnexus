using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public class RegisterDto
    {
            [Required]
            [MaxLength(20, ErrorMessage = "Username cannot exceed 20 characters.")]
            public string Username { get; set; } = "";

            [Required]
            [EmailAddress]
            [MaxLength(20, ErrorMessage = "Email cannot exceed 20 characters.")]
            public string Email { get; set; } = "";

            [Required]
            public string Password { get; set; } = "";
    }
}
