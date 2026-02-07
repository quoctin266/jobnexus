using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyEmployeeService
    {
        Task<ServiceResult<CompanyEmployee>> CreateAsync(CreateFormDto createFormDto, string userId);

        Task<ServiceResult<CompanyEmployee>> GetByIdAsync(int CompanyEmployeeId);
    }
}
