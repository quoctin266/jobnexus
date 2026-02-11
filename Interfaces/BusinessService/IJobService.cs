using JobNexus.Dtos.Job;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IJobService
    {
        Task<ServiceResult<Job>> CreateAsync(CreateJobDto createJobDto, ClaimsPrincipal user);

        Task<ServiceResult<Job>> FindById(int id);
    }
}
