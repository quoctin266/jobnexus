using JobNexus.Common.Enum;
using JobNexus.Dtos.User;

namespace JobNexus.Dtos.CompanyRequest
{
    public class CompanyRequestDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Address { get; set; } = "";

        public string Description { get; set; } = "";

        public string TIN { get; set; } = "";

        public string BusinessLicenseUrl { get; set; } = "";

        public string EmploymentContractUrl { get; set; } = "";

        public CompanyRequestStatus Status { get; set; }

        public string Reason { get; set; } = "";

        public UserDto? CreatedBy { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
