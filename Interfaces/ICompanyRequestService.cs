using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces
{
    public interface ICompanyRequestService
    {
        Task<CompanyRequest?> CreateRequestAsync(string userId, CreateCompanyRequestDto createCompanyRequestDto);

        Task<CompanyRequest?> CheckExistAsync(string userId);

        Task<CompanyRequest?> GetByIdAsync(int requestId);

        Task<QueryResponse<CompanyRequestDto>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal User);

    }
}
