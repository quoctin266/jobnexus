using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class CompanyEmployeeService : ICompanyEmployeeService
    {
        private readonly ApplicationDBContext _context;

        private readonly IBlobStorageService _blobStorageService;

        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;

        private readonly ICompanyRequestRepository _companyRequestRepository;

        private readonly ICompanyRepository _companyRepository;

        private readonly IAccountRepository _accountRepository;

        public CompanyEmployeeService(ApplicationDBContext context, IBlobStorageService blobStorageService, 
                                      ICompanyRepository companyRepository, IAccountRepository accountRepository,
                                      ICompanyRequestRepository companyRequestRepository, ICompanyEmployeeRepository companyEmployeeRepository)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _companyEmployeeRepository = companyEmployeeRepository;
            _companyRepository = companyRepository;
            _accountRepository = accountRepository;
            _companyRequestRepository = companyRequestRepository;
        }

        public async Task<ServiceResult<QueryResponse<CompanyEmployeeDto>>> GetAll(CompanyEmployeeQueryDto companyEmployeeQueryDto, ClaimsPrincipal user)
        {
            var data = await _companyEmployeeRepository.GetAllAsync(companyEmployeeQueryDto, user);

            return ServiceResult<QueryResponse<CompanyEmployeeDto>>.Success(new QueryResponse<CompanyEmployeeDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(cr => cr.ToCompanyEmployeeDto())
            });
        }

        public async Task<ServiceResult<CompanyEmployee>> GetById(int CompanyEmployeeId)
        {
            var companyEmployee = await _companyEmployeeRepository.GetByIdAsync(CompanyEmployeeId);

            if (companyEmployee == null)
            {
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound, 
                                                              [ErrorMessages.EmployeeNotFound]);
            }

            return ServiceResult<CompanyEmployee>.Success(companyEmployee);
        }

        public async Task<ServiceResult<CompanyEmployee>> Create(CreateFormDto createFormDto, string userId)
        {
            // Can only create new employees with non-owner roles
            if (createFormDto.CompanyRole == CompanyRole.Owner)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status400BadRequest,
                                                              Error.ViolatedRule,
                                                              [ErrorMessages.InvalidEmployeeRole]);

            // Ensure the company exists
            var company = await _companyRepository.GetByIdAsync(createFormDto.CompanyId);
            if (company == null) 
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.CompanyNotFound]);

            // Ensure the user being added as an employee exists
            var user = await _accountRepository.GetByIdAsync(createFormDto.AppUserId);
            if (user == null) 
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.UserNotFound]);

            var ownerEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);

            // Only company owners can add new employees
            if (ownerEmployment == null || ownerEmployment.CompanyRole != CompanyRole.Owner)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status403Forbidden,
                                                              Error.Forbidden,
                                                              [ErrorMessages.NoPermission]);

            // Ensure the user is being added to the same company as the owner
            if (ownerEmployment.CompanyId != createFormDto.CompanyId) 
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status400BadRequest,
                                                              Error.ViolatedRule,
                                                              [ErrorMessages.DifferentCompany]);

            
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(createFormDto.AppUserId);
            var userCompanyRequest = await _companyRequestRepository.CheckPendingAsync(createFormDto.AppUserId);

            // Ensure the user being added is not currently in another company or has a pending company request
            if (userEmployment != null || userCompanyRequest != null)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status400BadRequest,
                                                              Error.ViolatedRule,
                                                              [ErrorMessages.UserAlreadyEmployed]);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employmentContractUrl = await _blobStorageService.UploadFileAsync(createFormDto.EmploymentContract);

                var companyEmployee = new CompanyEmployee
                {
                    AppUserId = createFormDto.AppUserId,
                    CompanyId = createFormDto.CompanyId,
                    CompanyRole = createFormDto.CompanyRole,
                    EmploymentContractUrl = employmentContractUrl,
                };

                await _companyEmployeeRepository.CreateAsync(companyEmployee);

                var userRole = await _accountRepository.GetUserRoleAsync(user);
                if (userRole != Role.Employer.ToString())
                {
                    var updateRoleResult = await _accountRepository.UpdateUserRoleAsync(user, Role.Employer);

                    if (!updateRoleResult.Succeeded)
                    {
                        throw new Exception("Failed to update user role.");
                    }
                }

                await transaction.CommitAsync();

                return ServiceResult<CompanyEmployee>.Success(companyEmployee);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                await transaction.RollbackAsync();
            }

            return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status500InternalServerError,
                                                              Error.ServerFailure,
                                                              [ErrorMessages.ServerError]);
        }

        public async Task<ServiceResult<CompanyEmployee>> UpdateToInactive(int id, ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId!);

            // User must belong to a company
            if (userEmployment is null)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.ActiveEmploymentNotFound]);

            // Only company owners can update employee status
            if (userEmployment.CompanyRole != CompanyRole.Owner)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status403Forbidden,
                                                      Error.Forbidden,
                                                      [ErrorMessages.NoPermission]);

            var companyEmployee = await _companyEmployeeRepository.GetByIdAsync(id);
            if(companyEmployee is null)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status404NotFound,
                                                              Error.NotFound,
                                                              [ErrorMessages.EmployeeNotFound]);

            // Owners can only update employees within their own company
            if (userEmployment.CompanyId != companyEmployee.CompanyId)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status400BadRequest,
                                                              Error.ViolatedRule,
                                                              [ErrorMessages.EmployeeNotInCompany]);

            // Owners cannot update their own employment status
            if (userEmployment.Id == id)
                return ServiceResult<CompanyEmployee>.Failure(StatusCodes.Status400BadRequest,
                                                              Error.ViolatedRule,
                                                              [ErrorMessages.SelfUpdateNotAllowed]);

            await _companyEmployeeRepository.UpdateStatusAsync(companyEmployee, false);

            return ServiceResult<CompanyEmployee>.Success(companyEmployee);
        }
    }
}
