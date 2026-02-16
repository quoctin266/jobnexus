using JobNexus.Dtos.Company;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ICompanyRepository
    {
        Task<Company> UpdateAsync(Company company, UpdateCompanyDto updateCompanyDto);

        Task<Company> UpdateStatusAsync(Company company, bool IsActive);

        Task<Company> CreateAsync(CompanyRequest companyRequest);

        Task<Company?> GetByIdAsync(int companyId);

        Task<QueryResponse<Company>> GetAllAsync(CompanyQueryDto companyQueryDto);
    }
}
