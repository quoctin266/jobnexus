using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Dtos.Application;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;

        private readonly IJobRepository _jobRepository;

        private readonly IResumeVersionRepository _resumeVersionRepository;

        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;

        public ApplicationService(
            IApplicationRepository applicationRepository,
            IJobRepository jobRepository,
            IResumeVersionRepository resumeVersionRepository,
            ICompanyEmployeeRepository companyEmployeeRepository)
        {
            _applicationRepository = applicationRepository;
            _jobRepository = jobRepository;
            _resumeVersionRepository = resumeVersionRepository;
            _companyEmployeeRepository = companyEmployeeRepository;
        }

        public async Task<ServiceResult<Application>> Create(CreateApplicationDto createApplicationDto, ClaimsPrincipal user)
        {
            var userId = user.GetUserId()!;
            
            var job = await _jobRepository.GetByIdAsync(createApplicationDto.JobId);
            if (job == null)
                return ServiceResult<Application>.Failure(StatusCodes.Status404NotFound,
                                                  Error.NotFound,
                                                  [ErrorMessages.JobNotFound]);

            // Only allow to apply for approved jobs
            if (job.Status != JobStatus.Approved)
                return ServiceResult<Application>.Failure(StatusCodes.Status400BadRequest,
                                                  Error.ViolatedRule,
                                                  [ErrorMessages.ApplicationNotAllowed]);

            var resumeVersion = await _resumeVersionRepository.GetByIdAsync(createApplicationDto.ResumeVersionId);
            if(resumeVersion == null)
                return ServiceResult<Application>.Failure(StatusCodes.Status404NotFound,
                                                  Error.NotFound,
                                                  [ErrorMessages.ResumeNotFound]);

            // Only allow to use resume that belongs to the applicant
            if (resumeVersion.Resume!.AppUserId != userId)
                return ServiceResult<Application>.Failure(StatusCodes.Status400BadRequest,
                                                  Error.ViolatedRule,
                                                  [ErrorMessages.ResumeNotOwned]);

            var existingApplication = await _applicationRepository.CheckExistAsync(createApplicationDto.JobId, userId);
            if (existingApplication)
                return ServiceResult<Application>.Failure(StatusCodes.Status409Conflict,
                                                  Error.ResourceConflict,
                                                  [ErrorMessages.DuplicatedApplication]);

            try
            {
                var application = await _applicationRepository.CreateAsync(createApplicationDto, userId);

                return ServiceResult<Application>.Success(application);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return ServiceResult<Application>.Failure(StatusCodes.Status500InternalServerError,
                                                  Error.ServerFailure,
                                                  [ex.Message]);
            }
        }

        public async Task<ServiceResult<Application>> FindById(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            
            if (application == null)
                return ServiceResult<Application>.Failure(StatusCodes.Status404NotFound,
                                                  Error.NotFound,
                                                  [ErrorMessages.ApplicationNotFound]);

            return ServiceResult<Application>.Success(application);
        }

        public async Task<ServiceResult<QueryResponse<ApplicationDto>>> GetAll(ApplicationQueryDto applicationQueryDto, ClaimsPrincipal user)
        {
            var data = await _applicationRepository.GetAllAsync(applicationQueryDto, user);

            return ServiceResult<QueryResponse<ApplicationDto>>.Success(new QueryResponse<ApplicationDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(a => a.ToApplicationDto())
            });
        }

        public async Task<ServiceResult<Application>> UpdateStatus(int id, UpdateApplicationStatusDto updateApplicationStatusDto, 
                                                                   ClaimsPrincipal user)
        {
            // Only allow to update to accepted or rejected status
            if (updateApplicationStatusDto.Status == ApplicationStatus.Pending)
                return ServiceResult<Application>.Failure(StatusCodes.Status400BadRequest,
                                                  Error.ViolatedRule,
                                                  [ErrorMessages.InvalidApplicationStatusUpdate]);

            var application = await _applicationRepository.GetByIdAsync(id);
            if (application is null)
                return ServiceResult<Application>.Failure(StatusCodes.Status404NotFound,
                                                  Error.NotFound,
                                                  [ErrorMessages.ApplicationNotFound]);

            // Only allow to update status of pending application
            if (application.Status != ApplicationStatus.Pending)
                return ServiceResult<Application>.Failure(StatusCodes.Status400BadRequest,
                                                  Error.ViolatedRule,
                                                  [ErrorMessages.ApplicationUpdateNotAllowed]);

            var userId = user.GetUserId()!;
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);

            // Must be an active employee to update application status
            if (userEmployment is null)
                return ServiceResult<Application>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ActiveEmploymentNotFound]);

            // Only allow the employee that posted the job to update application status
            if (userEmployment.Id != application.Job?.CompanyEmployeeId)
                return ServiceResult<Application>.Failure(StatusCodes.Status403Forbidden,
                                                  Error.Forbidden,
                                                  [ErrorMessages.NoPermission]);

            await _applicationRepository.UpdateStatusAsync(application, updateApplicationStatusDto);

            return ServiceResult<Application>.Success(application);
        }
    }
}
