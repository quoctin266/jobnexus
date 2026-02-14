using JobNexus.Dtos.User;

namespace JobNexus.Dtos.Resume
{
    public record ResumeDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = "";

        public bool IsDefault { get; set; }

        public UserSummaryDto? CreatedBy { get; set; }

        public string FileUrl { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
