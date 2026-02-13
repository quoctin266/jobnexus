using JobNexus.Common.Enum;
using JobNexus.Dtos.Company;
using JobNexus.Dtos.User;

namespace JobNexus.Dtos.CompanyEmployee
{
    public record CompanyEmployeeSummaryDto
    {
        public int Id { get; init; }

        public CompanyRole CompanyRole { get; init; }

        public bool IsActive { get; init; }

        public UserSummaryDto? User { get; init; }
    }
}
