using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class CompanyEmployeeMappers
    {
        public static CompanyEmployeeDto ToCompanyEmployeeDto(this CompanyEmployee companyEmployee)
        {
            return new CompanyEmployeeDto
            {
                Id = companyEmployee.Id,
                CompanyRole = companyEmployee.CompanyRole,
                EmploymentContractUrl = companyEmployee.EmploymentContractUrl,
                IsActive = companyEmployee.IsActive,
                Company = companyEmployee.Company?.ToCompanyDto(),
                AppUser = companyEmployee.AppUser?.ToUserDto()
            };
        }
    }
}
