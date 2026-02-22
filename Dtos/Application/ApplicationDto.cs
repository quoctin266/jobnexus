using JobNexus.Common.Enum;
using JobNexus.Dtos.Job;
using JobNexus.Dtos.User;

namespace JobNexus.Dtos.Application
{
    public record ApplicationDto
    {
        public int Id { get; init; }

        public string PhoneNumber { get; init; } = "";

        public string FullName { get; init; } = "";

        public string Email { get; init; } = "";

        public string Intro { get; init; } = "";

        public JobDto? Job { get; init; }

        public string ResumeUrl { get; init; } = "";

        public UserSummaryDto? CreatedBy { get; init; }

        public ApplicationStatus Status { get; init; }

        public DateTimeOffset CreatedAt { get; init; }

        public DateTimeOffset UpdatedAt { get; init; }
    }
}
