namespace JobNexus.Dtos.Email
{
    public record PasswordResetDto
    {
        public string CompanyName { get; init; } = "JobNexus";

        public string Username { get; init; } = "";

        public string ResetUrl { get; init; } = "";
    }
}
