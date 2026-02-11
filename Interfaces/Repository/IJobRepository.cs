using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface IJobRepository
    {
        Task<Job> CreateAsync(Job job);

        Task<Job?> GetByIdAsync(int id);
    }
}
