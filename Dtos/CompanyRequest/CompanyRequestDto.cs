using JobNexus.Common.Enum;
using JobNexus.Dtos.User;

namespace JobNexus.Dtos.CompanyRequest
{
    public record CompanyRequestDto
    {
        public int Id { get; init; }

        public string Name { get; init; } = "";

        public string Address { get; init; } = "";

        public string Description { get; init; } = "";

        public string TIN { get; init; } = "";

        public string BusinessLicenseUrl { get; init; } = "";

        public string EmploymentContractUrl { get; init; } = "";

        public CompanyRequestStatus Status { get; init; }

        public string Reason { get; init; } = "";

        public UserDto? CreatedBy { get; init; }

        public DateTimeOffset CreatedAt { get; init; }
    }
}
