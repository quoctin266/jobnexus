using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyRequestService
    {
        Task<ServiceResult<CompanyRequest>> CreateRequestAsync(string userId, CreateCompanyRequestDto createCompanyRequestDto);

        Task<ServiceResult<CompanyRequest>> GetByIdAsync(int requestId);

        Task<ServiceResult<QueryResponse<CompanyRequestDto>>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal User);

        Task<ServiceResult<CompanyRequest>> UpdateStatusAsync(int requestId, UpdateCompanyRequestDto updateCompanyRequestDto);
    }
}
