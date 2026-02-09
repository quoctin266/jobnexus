using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public record LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; init; } = "";

        [Required]
        public string Password { get; init; } = "";

    }
}
