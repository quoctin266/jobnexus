using JobNexus.Dtos.CompanyRequest;
using JobNexus.Models;

namespace JobNexus.Interfaces
{
    public interface ICompanyRequestService
    {
        Task<CompanyRequest?> CreateRequestAsync(string userId, CreateCompanyRequestDto createCompanyRequestDto);

        Task<CompanyRequest?> CheckExistAsync(string userId);

        Task<CompanyRequest?> GetByIdAsync(int requestId);
    }
}
