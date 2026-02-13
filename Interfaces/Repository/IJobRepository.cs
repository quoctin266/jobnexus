using JobNexus.Dtos.Job;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface IJobRepository
    {
        Task<Job> CreateAsync(Job job);

        Task<Job> UpdateStatus(Job job, UpdateJobStatusDto updateJobStatusDto);

        Task<Job?> GetByIdAsync(int id);

        Task<QueryResponse<Job>> GetAllAsync(JobQueryDto jobQueryDto);
    }
}
