using JobNexus.Data;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;

namespace JobNexus.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDBContext _context;

        public JobRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Job> CreateAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return job;
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs.Include(j => j.Company)
                                      .Include(j => j.CompanyEmployee).ThenInclude(ce => ce!.AppUser)
                                      .Include(j => j.Skills)
                                      .FirstOrDefaultAsync(j => j.Id == id);
        }
    }
}
