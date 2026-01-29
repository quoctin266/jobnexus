using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Interfaces;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;

namespace JobNexus.Services.Business
{
    public class CompanyRequestService : ICompanyRequestService
    {
        private readonly IBlobStorageService _blobStorageService;
        private readonly ApplicationDBContext _context;

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

        public async Task<CompanyRequest?> GetByIdAsync(int requestId)
        {
            return await _context.CompanyRequests.Include(cr => cr.AppUser)
                .FirstOrDefaultAsync(cr => cr.Id == requestId);
        }
    }
}
