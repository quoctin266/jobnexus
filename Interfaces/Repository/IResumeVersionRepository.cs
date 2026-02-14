using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface IResumeVersionRepository
    {
        Task<ResumeVersion> CreateAsync(ResumeVersion resumeVersion);
    }
}
