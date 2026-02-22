using JobNexus.Dtos.Application;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.Repository
{
    public interface IApplicationRepository
    {
        Task<QueryResponse<Application>> GetAllAsync(ApplicationQueryDto applicationQueryDto, ClaimsPrincipal user);

        Task<Application?> GetByIdAsync(int id);

        Task<bool> CheckExistAsync(int jobId, string userId);

        Task<Application> CreateAsync(CreateApplicationDto createApplicationDto, string userId);

        Task<Application> UpdateStatusAsync(Application application, UpdateApplicationStatusDto updateApplicationStatusDto);
    }
}
