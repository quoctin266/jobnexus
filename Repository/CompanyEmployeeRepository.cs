using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace JobNexus.Repository
{
    public class CompanyEmployeeRepository : ICompanyEmployeeRepository
    {
        private readonly ApplicationDBContext _context;

        public CompanyEmployeeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        private readonly Dictionary<string, Expression<Func<CompanyEmployee, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["JobCount"] = ce => ce.Jobs.Count(),
            ["CreatedAt"] = ce => ce.CreatedAt,
        };

        public async Task<QueryResponse<CompanyEmployee>> GetAllAsync(CompanyEmployeeQueryDto companyEmployeeQueryDto, ClaimsPrincipal user)
        {
            var query = _context.CompanyEmployees.Include(ce => ce.Company)
                                                 .Include(ce => ce.AppUser)
                                                 .AsQueryable();

            if (companyEmployeeQueryDto.Role.HasValue)
            {
                query = query.Where(ce => ce.CompanyRole == companyEmployeeQueryDto.Role);
            }

            if (companyEmployeeQueryDto.IsActive.HasValue)
            {
                query = query.Where(ce => ce.IsActive == companyEmployeeQueryDto.IsActive);
            }

            // If admin is requesting, they can filter by any CompanyId
            if (user.IsInRole(Role.Admin.ToString()) && companyEmployeeQueryDto.CompanyId.HasValue)
            {
                query = query.Where(ce => ce.CompanyId == companyEmployeeQueryDto.CompanyId);
            }

            // If employer is requesting, they can only see their company's employees
            if (user.IsInRole(Role.Employer.ToString()))
            {
                var userId = user.GetUserId();
                var userEmployment = await _context.CompanyEmployees.FirstOrDefaultAsync(ce => ce.AppUserId == userId &&
                                                                                               ce.IsActive == true);

                if (userEmployment != null) 
                    query = query.Where(ce => ce.CompanyId == userEmployment.CompanyId);
            }

            // If admin is requesting, they can filter by any UserId
            if (user.IsInRole(Role.Admin.ToString()) && !string.IsNullOrWhiteSpace(companyEmployeeQueryDto.UserId))
            {
                query = query.Where(ce => ce.AppUserId == companyEmployeeQueryDto.UserId);
            }

            // If employer is requesting, they can only see their own employment record
            if (user.IsInRole(Role.Employer.ToString()))
            {
                query = query.Where(ce => ce.AppUserId == user.GetUserId());
            }

            if (!string.IsNullOrWhiteSpace(companyEmployeeQueryDto.SortBy))
            {
                query = query.ApplySorting(companyEmployeeQueryDto.SortBy, companyEmployeeQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)companyEmployeeQueryDto.PageSize);

            var offset = (companyEmployeeQueryDto.PageNumber - 1) * companyEmployeeQueryDto.PageSize;
            var items = await query.Skip(offset).Take(companyEmployeeQueryDto.PageSize).ToListAsync();

            return new QueryResponse<CompanyEmployee>
            {
                TotalPages = totalPages,
                PageNumber = companyEmployeeQueryDto.PageNumber,
                PageSize = companyEmployeeQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<CompanyEmployee?> GetActiveEmploymentAsync(string userId)
        {
            return await _context.CompanyEmployees.Include(ce => ce.Company)
                                                  .Include(ce => ce.AppUser)
                                                  .FirstOrDefaultAsync(ce => ce.AppUserId == userId && ce.IsActive == true);
        }

        public async Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId)
        {
            return await _context.CompanyEmployees.Include(ce => ce.Company)
                                                  .Include(ce => ce.AppUser)
                                                  .FirstOrDefaultAsync(ce => ce.Id == CompanyEmployeeId);
        }

        public async Task<CompanyEmployee> CreateAsync(CompanyEmployee companyEmployee)
        {
            await _context.CompanyEmployees.AddAsync(companyEmployee);
            await _context.SaveChangesAsync();

            return companyEmployee;
        }

        public async Task UpdateToInactiveAsync(int companyId)
        {
            await _context.CompanyEmployees.Where(ce => ce.CompanyId == companyId && ce.IsActive == true)
                                        .ForEachAsync(ce => ce.IsActive = false);

            await _context.SaveChangesAsync();
        }

        public async Task<CompanyEmployee> UpdateStatusAsync(CompanyEmployee companyEmployee, bool IsActive)
        {
            companyEmployee.IsActive = IsActive;

            await _context.SaveChangesAsync();

            return companyEmployee;
        }
    }
}
