using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Mappers;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class CompanyRequestService : ICompanyRequestService
    {
        private readonly IBlobStorageService _blobStorageService;

        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<CompanyRequest, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
           ["Name"] = cr => cr.Name,
           ["CreatedAt"] = cr => cr.CreatedAt,
           ["Status"] = cr => cr.Status
        };

        public CompanyRequestService(IBlobStorageService blobStorageService, ApplicationDBContext context) 
        {
            _blobStorageService = blobStorageService;
            _context = context;
        }

        public async Task<CompanyRequest?> CheckExistAsync(string userId)
        {
            return await _context.CompanyRequests
                .FirstOrDefaultAsync(cr => cr.AppUserId == userId && 
                    (cr.Status == CompanyRequestStatus.Pending || cr.Status == CompanyRequestStatus.Approved));
        }

        public async Task<CompanyRequest?> CreateRequestAsync(string userId, CreateCompanyRequestDto createCompanyRequestDto)
        {
            // Check if a pending or approved request already exists for the user
            if (await CheckExistAsync(userId) is not null)
            {
                return null;
            }

            var businessLicenseTask =  _blobStorageService.UploadFileAsync(createCompanyRequestDto.BusinessLicense);
            var employmentContracTask =  _blobStorageService.UploadFileAsync(createCompanyRequestDto.EmploymentContract);

            var CompanyRequest = new CompanyRequest
            {
                Name = createCompanyRequestDto.Name,
                Address = createCompanyRequestDto.Address,
                Description = createCompanyRequestDto.Description,
                TIN = createCompanyRequestDto.TIN,
                BusinessLicenseUrl = await businessLicenseTask,
                EmploymentContractUrl = await employmentContracTask,
                Status = CompanyRequestStatus.Pending,
                AppUserId = userId,
            };

            await _context.CompanyRequests.AddAsync(CompanyRequest);
            await _context.SaveChangesAsync();

            return CompanyRequest;
        }

        public async Task<QueryResponse<CompanyRequestDto>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal User)
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
            if (User.IsInRole(Role.Admin.ToString()) && !string.IsNullOrWhiteSpace(companyRequestQueryDto.UserId))
            {
                query = query.Where(cr => cr.AppUserId == companyRequestQueryDto.UserId);
            }

            // If regular user is requesting, they can only see their own requests
            if (User.IsInRole(Role.User.ToString()))
            {
                query = query.Where(cr => cr.AppUserId == User.GetUserId());
            }

            if (!string.IsNullOrWhiteSpace(companyRequestQueryDto.SortBy))
            {
                query = query.ApplySorting(companyRequestQueryDto.SortBy, companyRequestQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)companyRequestQueryDto.PageSize);

            var offset = (companyRequestQueryDto.PageNumber - 1) * companyRequestQueryDto.PageSize;
            var items = await query.Skip(offset).Take(companyRequestQueryDto.PageSize).ToListAsync();

            return new QueryResponse<CompanyRequestDto>
            {
                TotalPages = totalPages,
                PageNumber = companyRequestQueryDto.PageNumber,
                PageSize = companyRequestQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items.Select(cr => cr.ToCompanyRequestDto())
            };
        }

        public async Task<CompanyRequest?> GetByIdAsync(int requestId)
        {
            return await _context.CompanyRequests.Include(cr => cr.AppUser)
                .FirstOrDefaultAsync(cr => cr.Id == requestId);
        }
    }
}
