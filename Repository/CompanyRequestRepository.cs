using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace JobNexus.Repository
{
    public class CompanyRequestRepository : ICompanyRequestRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<CompanyRequest, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = cr => cr.Name,
            ["CreatedAt"] = cr => cr.CreatedAt,
            ["Status"] = cr => cr.Status
        };

        public CompanyRequestRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<CompanyRequest?> CheckPendingOrApprovedAsync(string userId)
        {
            return await _context.CompanyRequests
               .FirstOrDefaultAsync(cr => cr.AppUserId == userId &&
                   (cr.Status == CompanyRequestStatus.Pending || cr.Status == CompanyRequestStatus.Approved));
        }

        public async Task<QueryResponse<CompanyRequest>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, 
                                                                     ClaimsPrincipal user)
        {
            var query = _context.CompanyRequests.Include(cr => cr.AppUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(companyRequestQueryDto.CompanyName))
            {
                query = query.Where(cr => cr.Name.ToLower().Contains(companyRequestQueryDto.CompanyName.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(companyRequestQueryDto.TIN))
            {
                query = query.Where(cr => cr.TIN.Contains(companyRequestQueryDto.TIN));
            }

            if (companyRequestQueryDto.Status != null)
            {
                query = query.Where(cr => cr.Status == companyRequestQueryDto.Status);
            }

            // If admin is requesting, they can filter by any UserId
            if (user.IsInRole(Role.Admin.ToString()) && !string.IsNullOrWhiteSpace(companyRequestQueryDto.UserId))
            {
                query = query.Where(cr => cr.AppUserId == companyRequestQueryDto.UserId);
            }

            // If regular user is requesting, they can only see their own requests
            if (user.IsInRole(Role.User.ToString()))
            {
                query = query.Where(cr => cr.AppUserId == user.GetUserId());
            }

            if (!string.IsNullOrWhiteSpace(companyRequestQueryDto.SortBy))
            {
                query = query.ApplySorting(companyRequestQueryDto.SortBy, companyRequestQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)companyRequestQueryDto.PageSize);

            var offset = (companyRequestQueryDto.PageNumber - 1) * companyRequestQueryDto.PageSize;
            var items = await query.Skip(offset).Take(companyRequestQueryDto.PageSize).ToListAsync();

            return new QueryResponse<CompanyRequest>
            {
                TotalPages = totalPages,
                PageNumber = companyRequestQueryDto.PageNumber,
                PageSize = companyRequestQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<CompanyRequest?> GetByIdAsync(int requestId)
        {
            return await _context.CompanyRequests.Include(cr => cr.AppUser).FirstOrDefaultAsync(cr => cr.Id == requestId);
        }

        public async Task<CompanyRequest> CreateAsync(CreateCompanyRequestDto createCompanyRequestDto, string businessLicenseUrl, 
                                                      string employmentContracUrl, string userId)
        {
            var companyRequest = new CompanyRequest
            {
                Name = createCompanyRequestDto.Name,
                Address = createCompanyRequestDto.Address,
                Description = createCompanyRequestDto.Description,
                TIN = createCompanyRequestDto.TIN,
                BusinessLicenseUrl = businessLicenseUrl,
                EmploymentContractUrl = employmentContracUrl,
                Status = CompanyRequestStatus.Pending,
                AppUserId = userId,
            };

            await _context.CompanyRequests.AddAsync(companyRequest);
            await _context.SaveChangesAsync();

            await _context.Entry(companyRequest).Reference(cr => cr.AppUser).LoadAsync();

            return companyRequest;
        }

        public async Task<CompanyRequest> UpdateStatusAsync(CompanyRequest companyRequest, UpdateCompanyRequestDto updateCompanyRequestDto)
        {
            
            companyRequest.Status = updateCompanyRequestDto.Status;
            companyRequest.Reason = updateCompanyRequestDto.Reason;

            await _context.SaveChangesAsync();

            return companyRequest;
        }
    }
}
