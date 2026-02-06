using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Models;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyEmployeeService
    {
        Task<CompanyEmployee?> CreateAsync(CreateFormDto createFormDto, string userId);

        Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId);
    }
}
