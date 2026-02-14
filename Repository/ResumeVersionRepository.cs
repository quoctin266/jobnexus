using JobNexus.Data;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;

namespace JobNexus.Repository
{
    public class ResumeVersionRepository : IResumeVersionRepository
    {
        private readonly ApplicationDBContext _context;

        public ResumeVersionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ResumeVersion> CreateAsync(ResumeVersion resumeVersion)
        {
            await _context.ResumeVersions.AddAsync(resumeVersion);

            await _context.SaveChangesAsync();

            return resumeVersion;
        }
    }
}
