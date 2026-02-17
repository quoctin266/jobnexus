using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.Repository
{
    public interface ICompanyEmployeeRepository
    {
        Task<QueryResponse<CompanyEmployee>> GetAllAsync(CompanyEmployeeQueryDto companyEmployeeQueryDto, ClaimsPrincipal user);

        Task<CompanyEmployee?> GetActiveEmploymentAsync(string userId);

        Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId);

        Task<CompanyEmployee> UpdateStatusAsync(CompanyEmployee companyEmployee, bool IsActive);

        Task UpdateToInactiveAsync(int companyId);

        Task<CompanyEmployee> CreateAsync(CompanyEmployee companyEmployee);
    }
}
