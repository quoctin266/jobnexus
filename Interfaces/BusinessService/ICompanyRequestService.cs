using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ICompanyRequestService
    {
        Task<CompanyRequest?> CreateRequestAsync(string userId, CreateCompanyRequestDto createCompanyRequestDto);

        Task<CompanyRequest?> GetByIdAsync(int requestId);

        Task<QueryResponse<CompanyRequestDto>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal User);

        Task<CompanyRequest?> UpdateStatusAsync(int requestId, UpdateCompanyRequestDto updateCompanyRequestDto);
    }
}
