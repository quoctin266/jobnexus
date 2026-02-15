using JobNexus.Dtos.Job;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IJobService
    {
        Task<ServiceResult<QueryResponse<JobDto>>> GetAll(JobQueryDto jobQueryDto);

        Task<ServiceResult<Job>> UpdateStatus(int id, UpdateJobStatusDto updateJobStatusDto, ClaimsPrincipal user);

        Task<ServiceResult<Job>> Update(int id, UpdateJobDto updateJobDto, ClaimsPrincipal user);

        Task<ServiceResult<Job>> Create(CreateJobDto createJobDto, ClaimsPrincipal user);

        Task<ServiceResult<Job>> FindById(int id);
    }
}
