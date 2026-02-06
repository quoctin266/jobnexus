using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;

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

        public async Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId)
        {
            return await _companyEmployeeRepository.GetByIdAsync(CompanyEmployeeId);
        }

        public async Task<CompanyEmployee?> CreateAsync(CreateFormDto createFormDto, string userId)
        {
            // Can only create new employees with non-owner roles
            if (createFormDto.CompanyRole == CompanyRole.Owner) return null;

            // Ensure the company exists
            var company = await _companyRepository.GetByIdAsync(createFormDto.CompanyId);
            if (company == null) return null;

            // Ensure the user being added as an employee exists
            var user = await _accountRepository.GetByIdAsync(createFormDto.AppUserId);
            if (user == null) return null;

            var ownerEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);

            // Only company owners can add new employees
            if (ownerEmployment == null || ownerEmployment.CompanyRole != CompanyRole.Owner) return null;

            // Ensure the user is being added to the same company as the owner
            if (ownerEmployment.CompanyId != createFormDto.CompanyId) return null;

            // Ensure the user being added does not belong to another company or has a pending company request
            if (await _companyRequestRepository.CheckPendingOrApprovedAsync(createFormDto.AppUserId) is not null) return null;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var employmentContractUrl = await _blobStorageService.UploadFileAsync(createFormDto.EmploymentContract);

                var createCompanyEmployeeDto = new CreateCompanyEmployeeDto
                {
                    AppUserId = createFormDto.AppUserId,
                    CompanyId = createFormDto.CompanyId,
                    CompanyRole = createFormDto.CompanyRole,
                    EmploymentContractUrl = employmentContractUrl,
                };

                var companyEmployee = await _companyEmployeeRepository.CreateAsync(createCompanyEmployeeDto);

                await _accountRepository.UpdateUserRoleAsync(user, Role.Employer);

                await transaction.CommitAsync();

                return companyEmployee;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                await transaction.RollbackAsync();
            }

            return null;
        }
    }
}
