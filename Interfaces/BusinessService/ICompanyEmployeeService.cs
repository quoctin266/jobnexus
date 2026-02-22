using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyEmployeeService
    {
        Task<ServiceResult<QueryResponse<CompanyEmployeeDto>>> GetAll(CompanyEmployeeQueryDto companyEmployeeQueryDto, ClaimsPrincipal user);

        Task<ServiceResult<CompanyEmployee>> GetById(int CompanyEmployeeId);

        Task<ServiceResult<CompanyEmployee>> Create(CreateFormDto createFormDto, string userId);

        Task<ServiceResult<CompanyEmployee>> UpdateToInactive(int id, ClaimsPrincipal user);
    }
}
