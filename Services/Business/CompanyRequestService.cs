using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class CompanyRequestService : ICompanyRequestService
    {
        private readonly ApplicationDBContext _context;

        private readonly IBlobStorageService _blobStorageService;

        private readonly ICompanyRequestRepository _companyRequestRepository;

        private readonly ICompanyRepository _companyRepository;

        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;

        private readonly IAccountRepository _accountRepository;

        public CompanyRequestService(IBlobStorageService blobStorageService, ApplicationDBContext context, 
                                     ICompanyRequestRepository companyRequestRepository, ICompanyRepository companyRepository, 
                                     ICompanyEmployeeRepository companyEmployeeRepository, IAccountRepository accountRepository) 
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _companyRequestRepository = companyRequestRepository;
            _companyRepository = companyRepository;
            _companyEmployeeRepository = companyEmployeeRepository;
            _accountRepository = accountRepository;
        }

        public async Task<CompanyRequest?> CreateRequestAsync(string userId, CreateCompanyRequestDto createCompanyRequestDto)
        {
            // Check if a pending or approved request already exists for the user
            if (await _companyRequestRepository.CheckPendingOrApprovedAsync(userId) is not null)
            {
                return null;
            }

            var businessLicenseTask =  _blobStorageService.UploadFileAsync(createCompanyRequestDto.BusinessLicense);
            var employmentContracTask =  _blobStorageService.UploadFileAsync(createCompanyRequestDto.EmploymentContract);

            return await _companyRequestRepository.CreateAsync(createCompanyRequestDto, await businessLicenseTask, 
                                                               await employmentContracTask, userId);
        }

        public async Task<QueryResponse<CompanyRequestDto>> GetAllAsync(CompanyRequestQueryDto companyRequestQueryDto, ClaimsPrincipal user)
        {
            var data = await _companyRequestRepository.GetAllAsync(companyRequestQueryDto, user);

            return new QueryResponse<CompanyRequestDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(cr => cr.ToCompanyRequestDto())
            };
        }

        public async Task<CompanyRequest?> GetByIdAsync(int requestId)
        {
            return await _companyRequestRepository.GetByIdAsync(requestId);
        }

        public async Task<CompanyRequest?> UpdateStatusAsync(int requestId, UpdateCompanyRequestDto updateCompanyRequestDto)
        {
            if(updateCompanyRequestDto.Status == CompanyRequestStatus.Rejected)
            {
                   return await _companyRequestRepository.UpdateStatusAsync(requestId, updateCompanyRequestDto);
            }

            if (updateCompanyRequestDto.Status == CompanyRequestStatus.Approved)
            {
                 await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var companyRequest = await _companyRequestRepository.UpdateStatusAsync(requestId, updateCompanyRequestDto);

                    if(companyRequest != null)
                    {
                        var company = await _companyRepository.CreateAsync(companyRequest);

                        var createCompanyEmployeeDto = new CreateCompanyEmployeeDto
                        {
                            EmploymentContractUrl = companyRequest.EmploymentContractUrl,
                            CompanyId = company.Id,
                            AppUserId = companyRequest.AppUserId,
                            CompanyRole = CompanyRole.Owner,
                        };

                        await _companyEmployeeRepository.CreateAsync(createCompanyEmployeeDto);

                        var user = await _accountRepository.GetByIdAsync(companyRequest.AppUserId);

                        var updateRoleResult = await _accountRepository.UpdateUserRoleAsync(user!, Role.Employer);

                        if (!updateRoleResult.Succeeded)
                        {
                            throw new Exception("Failed to update user role.");
                        }
                    }

                    await transaction.CommitAsync();

                    return companyRequest;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);

                    await transaction.RollbackAsync();
                }
            }

            return null;
        }
    }
}
