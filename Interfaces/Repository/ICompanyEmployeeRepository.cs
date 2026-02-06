using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ICompanyEmployeeRepository
    {
        Task<CompanyEmployee> CreateAsync(CreateCompanyEmployeeDto createCompanyEmployeeDto);

        Task<CompanyEmployee?> GetActiveEmploymentAsync(string userId);

        Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId);
    }
}
