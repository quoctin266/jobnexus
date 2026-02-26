namespace JobNexus.Dtos.Email
{
    public record ConfirmEmailDto
    {
        public string ConfirmationUrl { get; init; } = "";

        public string CompanyName { get; init; } = "JobNexus";

        public string CompanyUrl { get; init; } = "https://jobnexus-api.onrender.com/swagger";
    }
}
