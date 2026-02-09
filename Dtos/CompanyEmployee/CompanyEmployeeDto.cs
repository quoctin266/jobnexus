using JobNexus.Common.Enum;
using JobNexus.Dtos.Company;
using JobNexus.Dtos.User;

namespace JobNexus.Dtos.CompanyEmployee
{
    public record CompanyEmployeeDto
    {
        public int Id { get; init; }

        public CompanyRole CompanyRole { get; init; }

        public string EmploymentContractUrl { get; init; } = "";

        public bool IsActive { get; init; }

        public CompanyDto? Company { get; init; }

        public UserDto? AppUser { get; init; }
    }
}
