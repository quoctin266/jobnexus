using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.Repository
{
    public interface ICompanyRequestRepository
    {
        Task<CompanyRequest?> CheckPendingAsync(string userId);

        Task<CompanyRequest?> GetByIdAsync(int requestId);

        Task<QueryResponse<CompanyRequest>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal user);

        Task<CompanyRequest> CreateAsync(CreateCompanyRequestDto createCompanyRequestDto, string businessLicenseUrl, string employmentContracUrl, string userId);

        Task<CompanyRequest> UpdateStatusAsync(CompanyRequest companyRequest, UpdateCompanyRequestDto updateCompanyRequestDto);
    }
}
