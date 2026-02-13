namespace JobNexus.Dtos.User
{
    public record UserSummaryDto
    {
        public string Id { get; init; } = "";

        public string Username { get; init; } = "";

        public string Email { get; init; } = "";
    }
}
