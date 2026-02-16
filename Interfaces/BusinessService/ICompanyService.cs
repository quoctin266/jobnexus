using JobNexus.Dtos.Company;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyService
    {
        Task<ServiceResult<QueryResponse<CompanyDto>>> GetAll(CompanyQueryDto companyQueryDto);

        Task<ServiceResult<Company>> FindById(int id);

        Task<ServiceResult<Company>> Update(int id, UpdateCompanyDto updateCompanyDto, ClaimsPrincipal user);

        Task<ServiceResult<Company>> UpdateToInactive(int id, ClaimsPrincipal user);
    }
}
