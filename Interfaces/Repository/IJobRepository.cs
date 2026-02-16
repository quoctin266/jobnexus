using JobNexus.Dtos.Job;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface IJobRepository
    {
        Task<Job> CreateAsync(Job job);

        Task<Job> UpdateStatusAsync(Job job, UpdateJobStatusDto updateJobStatusDto);

        Task<Job> UpdateAsync(Job job, UpdateJobDto updateJobDto, IEnumerable<Skill> skills);

        Task<Job?> GetByIdAsync(int id);

        Task<QueryResponse<Job>> GetAllAsync(JobQueryDto jobQueryDto);

        Task UpdateToClosedAsync(int companyId);
    }
}
