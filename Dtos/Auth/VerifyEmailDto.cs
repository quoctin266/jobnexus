using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Auth
{
    public record VerifyEmailDto
    {
        [Required]
        public string Token { get; init; } = "";
    }
}
