using JobNexus.Dtos.Application;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IApplicationService
    {
        Task<ServiceResult<QueryResponse<ApplicationDto>>> GetAll(ApplicationQueryDto applicationQueryDto, ClaimsPrincipal user);

        Task<ServiceResult<Application>> FindById(int id);

        Task<ServiceResult<Application>> Create(CreateApplicationDto createApplicationDto, ClaimsPrincipal user);

        Task<ServiceResult<Application>> UpdateStatus(int id, UpdateApplicationStatusDto updateApplicationStatusDto, ClaimsPrincipal user);
    }
}
