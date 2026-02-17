using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
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

        public async Task<ServiceResult<QueryResponse<CompanyRequestDto>>> GetAll(CompanyRequestQueryDto companyRequestQueryDto, 
                                                                                       ClaimsPrincipal user)
        {
            var data = await _companyRequestRepository.GetAllAsync(companyRequestQueryDto, user);

            return ServiceResult<QueryResponse<CompanyRequestDto>>.Success(new QueryResponse<CompanyRequestDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(cr => cr.ToCompanyRequestDto())
            });
        }

        public async Task<ServiceResult<CompanyRequest>> GetById(int requestId)
        {
            var companyRequest = await _companyRequestRepository.GetByIdAsync(requestId);

            if(companyRequest is null)
            {
                return ServiceResult<CompanyRequest>.Failure(StatusCodes.Status404NotFound, 
                                                             Error.NotFound, [ErrorMessages.CompanyRequestNotFound]);
            }

            return ServiceResult<CompanyRequest>.Success(companyRequest);
        }

        public async Task<ServiceResult<CompanyRequest>> Create(string userId, CreateCompanyRequestDto createCompanyRequestDto)
        {
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);
            var userRequest = await _companyRequestRepository.CheckPendingAsync(userId);

            // Can only create company request when user isn't currently in a company or have pending request
            if (userEmployment is not null || userRequest is not null)
            {
                return ServiceResult<CompanyRequest>.Failure(StatusCodes.Status409Conflict,
                                                             Error.ResourceConflict,
                                                             [ErrorMessages.CompanyRequestConflict]);
            }

            var businessLicenseTask = _blobStorageService.UploadFileAsync(createCompanyRequestDto.BusinessLicense);
            var employmentContractTask = _blobStorageService.UploadFileAsync(createCompanyRequestDto.EmploymentContract);

            var companyRequest = await _companyRequestRepository.CreateAsync(createCompanyRequestDto, await businessLicenseTask,
                                                               await employmentContractTask, userId);

            return ServiceResult<CompanyRequest>.Success(companyRequest);
        }

        public async Task<ServiceResult<CompanyRequest>> UpdateStatus(int requestId, UpdateCompanyRequestDto updateCompanyRequestDto)
        {
            if(updateCompanyRequestDto.Status == CompanyRequestStatus.Pending)
            {
                return ServiceResult<CompanyRequest>.Failure(StatusCodes.Status400BadRequest,
                                                             Error.ViolatedRule,
                                                             [ErrorMessages.InvalidCompanyRequestStatusUpdate]);
            }

            var companyRequest = await _companyRequestRepository.GetByIdAsync(requestId);

            if(companyRequest is null)
            {
                return ServiceResult<CompanyRequest>.Failure(StatusCodes.Status404NotFound,
                                                             Error.NotFound,
                                                             [ErrorMessages.CompanyRequestNotFound]);
            }

            if (updateCompanyRequestDto.Status == CompanyRequestStatus.Rejected)
            {
                await _companyRequestRepository.UpdateStatusAsync(companyRequest, updateCompanyRequestDto);

                return ServiceResult<CompanyRequest>.Success(companyRequest);
            }

            if (updateCompanyRequestDto.Status == CompanyRequestStatus.Approved)
            {
                 await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    await _companyRequestRepository.UpdateStatusAsync(companyRequest, updateCompanyRequestDto);

                    var company = await _companyRepository.CreateAsync(companyRequest);

                    var companyEmployee = new CompanyEmployee
                    {
                        EmploymentContractUrl = companyRequest.EmploymentContractUrl,
                        CompanyId = company.Id,
                        AppUserId = companyRequest.AppUserId,
                        CompanyRole = CompanyRole.Owner,
                    };

                    await _companyEmployeeRepository.CreateAsync(companyEmployee);
                       
                    var user = await _accountRepository.GetByIdAsync(companyRequest.AppUserId) ?? throw new Exception("Not found user with providid id.");
                    var userRole = await _accountRepository.GetUserRoleAsync(user);

                    if(userRole != Role.Employer.ToString())
                    {
                        var updateRoleResult = await _accountRepository.UpdateUserRoleAsync(user, Role.Employer);

                        if (!updateRoleResult.Succeeded)
                        {
                            throw new Exception("Failed to update user role.");
                        }
                    }
                    
                    await transaction.CommitAsync();

                    return ServiceResult<CompanyRequest>.Success(companyRequest);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);

                    await transaction.RollbackAsync();
                }
            }

            return ServiceResult<CompanyRequest>.Failure(StatusCodes.Status500InternalServerError,
                                                         Error.ServerFailure,
                                                         [ErrorMessages.ServerError]);
        }
    }
}
