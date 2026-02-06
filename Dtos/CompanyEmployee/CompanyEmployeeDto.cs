using JobNexus.Common.Enum;
using JobNexus.Dtos.Company;
using JobNexus.Dtos.User;

namespace JobNexus.Dtos.CompanyEmployee
{
    public class CompanyEmployeeDto
    {
        public int Id { get; set; }

        public CompanyRole CompanyRole { get; set; }

        public string EmploymentContractUrl { get; set; } = "";

        public bool IsActive { get; set; }

        public CompanyDto? Company { get; set; }

        public UserDto? AppUser { get; set; }
    }
}
