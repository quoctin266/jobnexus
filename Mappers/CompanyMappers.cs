using JobNexus.Dtos.Company;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class CompanyMappers
    {
        public static CompanyDto ToCompanyDto(this Company company)
        {
            return new CompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                Description = company.Description,
                BusinessLicenseUrl = company.BusinessLicenseUrl,
                TIN = company.TIN,
                CreatedAt = company.CreatedAt,
                UpdatedAt = company.UpdatedAt
            };
        }

        public static CompanySummaryDto ToCompanySummaryDto(this Company company)
        {
            return new CompanySummaryDto
            {
                Id = company.Id,
                Name = company.Name,
                Address = company.Address,
                Description = company.Description,
                TIN = company.TIN
            };
        }
    }
}
