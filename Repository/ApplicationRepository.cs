using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.Application;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace JobNexus.Repository
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<Application, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["FullName"] = j => j.FullName,
            ["Email"] = j => j.Email,
            ["CreatedAt"] = j => j.CreatedAt,
            ["UpdatedAt"] = j => j.UpdatedAt,
        };

        public ApplicationRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse<Application>> GetAllAsync(ApplicationQueryDto applicationQueryDto, ClaimsPrincipal user)
        {
            var query = _context.Applications.Include(a => a.Job).ThenInclude(j => j.Company)
                                             .Include(a => a.ResumeVersion)
                                             .Include(a => a.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(applicationQueryDto.FullName))
            {
                query = query.Where(a => a.FullName.ToLower().Contains(applicationQueryDto.FullName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(applicationQueryDto.PhoneNumber))
            {
                query = query.Where(a => a.PhoneNumber.Contains(applicationQueryDto.PhoneNumber));
            }

            if (!string.IsNullOrWhiteSpace(applicationQueryDto.Email))
            {
                query = query.Where(a => a.Email.ToLower().Contains(applicationQueryDto.Email.ToLower()));
            }

            // If admin is requesting, they can filter by any UserId
            if (user.IsInRole(Role.Admin.ToString()) && !string.IsNullOrWhiteSpace(applicationQueryDto.UserId))
            {
                query = query.Where(a => a.AppUserId == applicationQueryDto.UserId);
            }

            // If regular user is requesting, they can only see their own applications
            if (user.IsInRole(Role.User.ToString()))
            {
                query = query.Where(a => a.AppUserId == user.GetUserId());
            }

            // If admin is requesting, they can filter by any JobId
            if (user.IsInRole(Role.Admin.ToString()) && applicationQueryDto.JobId.HasValue)
            {
                query = query.Where(a => a.JobId == applicationQueryDto.JobId);
            }

            // If employer is requesting, they can only see applications for their company's jobs
            if (user.IsInRole(Role.Employer.ToString()))
            {
                var userId = user.GetUserId();
                var userEmployment = await _context.CompanyEmployees.FirstOrDefaultAsync(ce => ce.AppUserId == userId &&
                                                                                               ce.IsActive == true);
                if (userEmployment != null)
                    query = query.Where(a => a.Job!.CompanyId == userEmployment.CompanyId);
            }

            if (applicationQueryDto.Status.HasValue)
            {
                query = query.Where(a => a.Status == applicationQueryDto.Status);
            }

            if (!string.IsNullOrWhiteSpace(applicationQueryDto.SortBy))
            {
                query = query.ApplySorting(applicationQueryDto.SortBy, applicationQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)applicationQueryDto.PageSize);

            var offset = (applicationQueryDto.PageNumber - 1) * applicationQueryDto.PageSize;
            var items = await query.Skip(offset).Take(applicationQueryDto.PageSize).ToListAsync();

            return new QueryResponse<Application>
            {
                TotalPages = totalPages,
                PageNumber = applicationQueryDto.PageNumber,
                PageSize = applicationQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            return await _context.Applications.Include(a => a.Job).ThenInclude(j => j.Company)
                                      .Include(a => a.ResumeVersion)
                                      .Include(a => a.AppUser)
                                      .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<bool> CheckExistAsync(int jobId, string userId)
        {
            var application = await _context.Applications.FirstOrDefaultAsync(a => a.JobId == jobId && a.AppUserId == userId);

            return application != null;
        }

        public async Task<Application> CreateAsync(CreateApplicationDto createApplicationDto, string userId)
        {
            var application = new Application
            {
                PhoneNumber = createApplicationDto.PhoneNumber,
                FullName = createApplicationDto.FullName,
                Email = createApplicationDto.Email,
                Intro = createApplicationDto.Intro ?? "",
                JobId = createApplicationDto.JobId,
                ResumeVersionId = createApplicationDto.ResumeVersionId,
                AppUserId = userId,
                Status = ApplicationStatus.Pending,
            };

            await _context.Applications.AddAsync(application);

            await _context.SaveChangesAsync();

            return application;
        }

        public async Task<Application> UpdateStatusAsync(Application application, UpdateApplicationStatusDto updateApplicationStatusDto)
        {
            application.Status = updateApplicationStatusDto.Status;

            await _context.SaveChangesAsync();

            return application;
        }
    }
}
