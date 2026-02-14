using JobNexus.Data;
using JobNexus.Dtos.Resume;
using JobNexus.Common.Enum;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace JobNexus.Repository
{
    public class ResumeRepository : IResumeRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<Resume, object>>> _sortMap =
            new(StringComparer.OrdinalIgnoreCase)
        {
            ["Title"] = r => r.Title,
            ["CreatedAt"] = r => r.CreatedAt,
            ["UpdatedAt"] = r => r.UpdatedAt,
        };

        public ResumeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Resume?> GetByIdAsync(int id)
        {
            var resume = await _context.Resumes
                                       .Include(r => r.AppUser)
                                       .FirstOrDefaultAsync(r => r.Id == id);

            if (resume == null)
                return null;

            // Load only the ResumeVersion with the highest VersionNo
            await _context.Entry(resume)
                          .Collection(r => r.ResumeVersions)
                          .Query()
                          .OrderByDescending(rv => rv.VersionNo)
                          .Take(1)
                          .LoadAsync();

            return resume;
        }

        public async Task<QueryResponse<Resume>> GetAllAsync(ResumeQueryDto resumeQueryDto, ClaimsPrincipal user)
        {
            var query = _context.Resumes.Include(r => r.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(resumeQueryDto.Title))
            {
                query = query.Where(r => r.Title.ToLower().Contains(resumeQueryDto.Title.ToLower()));
            }

            // If admin is requesting, they can filter by any UserId
            if (user.IsInRole(Role.Admin.ToString()) && !string.IsNullOrWhiteSpace(resumeQueryDto.UserId))
            {
                query = query.Where(r => r.AppUserId == resumeQueryDto.UserId);
            }

            // If regular user is requesting, they can only see their own resumes
            if (user.IsInRole(Role.User.ToString()))
            {
                query = query.Where(r => r.AppUserId == user.GetUserId());
            }

            if (!string.IsNullOrWhiteSpace(resumeQueryDto.SortBy))
            {
                query = query.ApplySorting(resumeQueryDto.SortBy, resumeQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)resumeQueryDto.PageSize);

            var offset = (resumeQueryDto.PageNumber - 1) * resumeQueryDto.PageSize;
            var items = await query.Skip(offset).Take(resumeQueryDto.PageSize).ToListAsync();
           
            // For each resume load only the ResumeVersion with the highest VersionNo
            foreach (var item in items)
            {
                await _context.Entry(item)
                              .Collection(r => r.ResumeVersions)
                              .Query()
                              .OrderByDescending(rv => rv.VersionNo)
                              .Take(1)
                              .LoadAsync();
            }

            return new QueryResponse<Resume>
            {
                TotalPages = totalPages,
                PageNumber = resumeQueryDto.PageNumber,
                PageSize = resumeQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<Resume> CreateAsync(CreateResumeDto createResumeDto, string userId)
        {
            var resume = new Resume
            {
                Title = createResumeDto.Title,
                IsDefault = createResumeDto.IsDefault,
                AppUserId = userId,
            };

            await _context.Resumes.AddAsync(resume);
            await _context.SaveChangesAsync();

            await _context.Entry(resume).Reference(r => r.AppUser).LoadAsync();

            return resume;
        }

        public async Task<Resume> UpdateAsync(Resume resume, UpdateResumeDto updateResumeDto)
        {
            resume.Title = updateResumeDto.Title;
            resume.IsDefault = updateResumeDto.IsDefault;

            await _context.SaveChangesAsync();

            return resume;
        }

        public async Task UpdateDefaultAsync(int defaultResumeId)
        {
            await _context.Resumes.Where(r => r.Id != defaultResumeId)
                                  .ForEachAsync(r => r.IsDefault = false);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Resume resume)
        {
            _context.Resumes.Remove(resume);
            await _context.SaveChangesAsync();
        }

      
    }
}
