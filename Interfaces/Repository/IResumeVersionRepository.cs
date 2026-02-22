using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface IResumeVersionRepository
    {
        Task<ResumeVersion?> GetByIdAsync(int id);

        Task<ResumeVersion> CreateAsync(ResumeVersion resumeVersion);
    }
}
