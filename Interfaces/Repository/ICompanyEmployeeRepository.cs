using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ICompanyEmployeeRepository
    {
        Task<CompanyEmployee> CreateAsync(CompanyEmployee companyEmployee);

        Task<CompanyEmployee?> GetActiveEmploymentAsync(string userId);

        Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId);

        Task UpdateToInactiveAsync(int companyId);
    }
}
