using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyRequestService
    {
        Task<ServiceResult<CompanyRequest>> Create(string userId, CreateCompanyRequestDto createCompanyRequestDto);

        Task<ServiceResult<CompanyRequest>> GetById(int requestId);

        Task<ServiceResult<QueryResponse<CompanyRequestDto>>> GetAll(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal User);

        Task<ServiceResult<CompanyRequest>> UpdateStatus(int requestId, UpdateCompanyRequestDto updateCompanyRequestDto);
    }
}
