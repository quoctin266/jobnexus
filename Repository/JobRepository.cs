using JobNexus.Data;
using JobNexus.Dtos.Job;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobNexus.Repository
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<Job, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = j => j.Name,
            ["Location"] = j => j.Location,
            ["Level"] = j => j.Level,
            ["SalaryMin"] = j => j.SalaryMin,
            ["SalaryMax"] = j => j.SalaryMax,
            ["Quantity"] = j => j.Quantity,
            ["StartDate"] = j => j.StartDate,
            ["EndDate"] = j => j.EndDate,
            ["CreatedAt"] = j => j.CreatedAt,
        };

        public JobRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse<Job>> GetAllAsync(JobQueryDto jobQueryDto)
        {
            var query = _context.Jobs.Include(j => j.Company)
                                      .Include(j => j.CompanyEmployee).ThenInclude(ce => ce!.AppUser)
                                      .Include(j => j.Skills).AsQueryable();

            if (!string.IsNullOrWhiteSpace(jobQueryDto.Name))
            {
                query = query.Where(j => j.Name.ToLower().Contains(jobQueryDto.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(jobQueryDto.Location))
            {
                query = query.Where(j => j.Location.ToLower().Contains(jobQueryDto.Location.ToLower()));
            }

            if (jobQueryDto.SalaryMin.HasValue)
            {
                query = query.Where(j => j.SalaryMin >= jobQueryDto.SalaryMin);
            }

            if (jobQueryDto.SalaryMax.HasValue)
            {
                query = query.Where(j => j.SalaryMax <= jobQueryDto.SalaryMax);
            }

            if (jobQueryDto.Quantity.HasValue)
            {
                query = query.Where(j => j.Quantity == jobQueryDto.Quantity);
            }

            if (!string.IsNullOrWhiteSpace(jobQueryDto.Level))
            {
                query = query.Where(j => j.Level.ToLower().Contains(jobQueryDto.Level.ToLower()));
            }

            if (jobQueryDto.StartDate.HasValue)
            {
                query = query.Where(j => j.StartDate >= jobQueryDto.StartDate);
            }

            if (jobQueryDto.EndDate.HasValue)
            {
                query = query.Where(j => j.EndDate <= jobQueryDto.EndDate);
            }

            if (jobQueryDto.SkillIds != null && jobQueryDto.SkillIds.Count > 0)
            {
                query = query.Where(j => j.Skills.Any(sk => jobQueryDto.SkillIds.Contains(sk.Id)));
            }

            if (jobQueryDto.CompanyId.HasValue)
            {
                query = query.Where(j => j.CompanyId == jobQueryDto.CompanyId);
            }

            if (jobQueryDto.CompanyEmployeeId.HasValue)
            {
                query = query.Where(j => j.CompanyEmployeeId == jobQueryDto.CompanyEmployeeId);
            }

            if (jobQueryDto.Status.HasValue)
            {
                query = query.Where(j => j.Status == jobQueryDto.Status);
            }

            if (!string.IsNullOrWhiteSpace(jobQueryDto.SortBy))
            {
                query = query.ApplySorting(jobQueryDto.SortBy, jobQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)jobQueryDto.PageSize);

            var offset = (jobQueryDto.PageNumber - 1) * jobQueryDto.PageSize;
            var items = await query.Skip(offset).Take(jobQueryDto.PageSize).ToListAsync();

            return new QueryResponse<Job>
            {
                TotalPages = totalPages,
                PageNumber = jobQueryDto.PageNumber,
                PageSize = jobQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<Job?> GetByIdAsync(int id)
        {
            return await _context.Jobs.Include(j => j.Company)
                                      .Include(j => j.CompanyEmployee).ThenInclude(ce => ce!.AppUser)
                                      .Include(j => j.Skills)
                                      .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<Job> CreateAsync(Job job)
        {
            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            return job;
        }

        public async Task<Job> UpdateStatusAsync(Job job, UpdateJobStatusDto updateJobStatusDto)
        {
            job.Status = updateJobStatusDto.Status;

            await _context.SaveChangesAsync();

            return job;
        }

        public async Task<Job> UpdateAsync(Job job, UpdateJobDto updateJobDto, IEnumerable<Skill> skills)
        {
            job.Name = updateJobDto.Name;
            job.Location = updateJobDto.Location;
            job.Level = updateJobDto.Level;
            job.SalaryMin = updateJobDto.SalaryMin;
            job.SalaryMax = updateJobDto.SalaryMax;
            job.Quantity = updateJobDto.Quantity;
            job.StartDate = updateJobDto.StartDate;
            job.EndDate = updateJobDto.EndDate;
            job.Description = updateJobDto.Description;
            job.Skills = [.. skills];

            await _context.SaveChangesAsync();

            return job;
        }
    }
}
