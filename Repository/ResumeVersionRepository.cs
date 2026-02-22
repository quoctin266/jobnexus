using JobNexus.Data;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;

namespace JobNexus.Repository
{
    public class ResumeVersionRepository : IResumeVersionRepository
    {
        private readonly ApplicationDBContext _context;

        public ResumeVersionRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<ResumeVersion?> GetByIdAsync(int id)
        {
            // rv.Resume might be null if the associated Resume has been deleted
            return await _context.ResumeVersions.Include(rv => rv.Resume).FirstOrDefaultAsync(rv => rv.Id == id);
        }

        public async Task<ResumeVersion> CreateAsync(ResumeVersion resumeVersion)
        {
            await _context.ResumeVersions.AddAsync(resumeVersion);

            await _context.SaveChangesAsync();

            return resumeVersion;
        }
    }
}
