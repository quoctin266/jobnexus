using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.Company;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class CompanyService : ICompanyService
    {
        private readonly ApplicationDBContext _context;

        private readonly ICompanyRepository _companyRepository;

        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;

        private readonly IJobRepository _jobRepository;

        public CompanyService(ApplicationDBContext context, ICompanyRepository companyRepository, 
                              ICompanyEmployeeRepository companyEmployeeRepository, IJobRepository jobRepository)
        {
            _context = context;
            _companyRepository = companyRepository;
            _companyEmployeeRepository = companyEmployeeRepository;
            _jobRepository = jobRepository;
        }

        public async Task<ServiceResult<Company>> FindById(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);

            if (company is null)
                return ServiceResult<Company>.Failure(StatusCodes.Status404NotFound,
                                                      Error.NotFound,
                                                      [ErrorMessages.CompanyNotFound]);

            return ServiceResult<Company>.Success(company);
        }

        public async Task<ServiceResult<QueryResponse<CompanyDto>>> GetAll(CompanyQueryDto companyQueryDto)
        {
            var data = await _companyRepository.GetAllAsync(companyQueryDto);

            return ServiceResult<QueryResponse<CompanyDto>>.Success(new QueryResponse<CompanyDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(j => j.ToCompanyDto())
            });
        }

        public async Task<ServiceResult<Company>> Update(int id, UpdateCompanyDto updateCompanyDto, ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId!);

            // User must be in a company to update company information
            if (userEmployment is null)
                return ServiceResult<Company>.Failure(StatusCodes.Status404NotFound,
                                                      Error.NotFound,
                                                      [ErrorMessages.ActiveEmploymentNotFound]);

            // User must be owner to update company information
            if (userEmployment.CompanyRole != CompanyRole.Owner)
                return ServiceResult<Company>.Failure(StatusCodes.Status403Forbidden,
                                                      Error.Forbidden,
                                                      [ErrorMessages.NoPermission]);

            var company = await _companyRepository.GetByIdAsync(id);
            if(company is null)
                return ServiceResult<Company>.Failure(StatusCodes.Status404NotFound,
                                                      Error.NotFound,
                                                      [ErrorMessages.CompanyNotFound]);

            // User can only update their own company information
            if (userEmployment.CompanyId != id)
                return ServiceResult<Company>.Failure(StatusCodes.Status403Forbidden,
                                                      Error.Forbidden,
                                                      [ErrorMessages.NoPermission]);

            await _companyRepository.UpdateAsync(company, updateCompanyDto);

            return ServiceResult<Company>.Success(company);
        }

        public async Task<ServiceResult<Company>> UpdateToInactive(int id, ClaimsPrincipal user)
        {
            var userId = user.GetUserId();
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId!);

            // User must be in a company to close that company
            if (userEmployment is null)
                return ServiceResult<Company>.Failure(StatusCodes.Status404NotFound,
                                                      Error.NotFound,
                                                      [ErrorMessages.ActiveEmploymentNotFound]);

            // User must be owner to close the company
            if (userEmployment.CompanyRole != CompanyRole.Owner)
                return ServiceResult<Company>.Failure(StatusCodes.Status403Forbidden,
                                                      Error.Forbidden,
                                                      [ErrorMessages.NoPermission]);

            var company = await _companyRepository.GetByIdAsync(id);
            if (company is null)
                return ServiceResult<Company>.Failure(StatusCodes.Status404NotFound,
                                                      Error.NotFound,
                                                      [ErrorMessages.CompanyNotFound]);

            // User can only close their own company
            if (userEmployment.CompanyId != id)
                return ServiceResult<Company>.Failure(StatusCodes.Status403Forbidden,
                                                      Error.Forbidden,
                                                      [ErrorMessages.NoPermission]);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // When a company is closed, all of its jobs are closed and all of its employees become inactive
                await _jobRepository.UpdateToClosedAsync(id);
                await _companyEmployeeRepository.UpdateToInactiveAsync(id);
                await _companyRepository.UpdateStatusAsync(company, false);

                await transaction.CommitAsync();

                return ServiceResult<Company>.Success(company);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                await transaction.RollbackAsync();
            }

            return ServiceResult<Company>.Failure(StatusCodes.Status500InternalServerError,
                                                  Error.ServerFailure,
                                                  [ErrorMessages.ServerError]);
        }
    }
}
