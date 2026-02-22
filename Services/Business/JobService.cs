using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.Job;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Services.Business
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        private readonly ISkillRepository _skillRepository;

        private readonly ICompanyEmployeeRepository _companyEmployeeRepository;

        public JobService(
            IJobRepository jobRepository,
            ISkillRepository skillRepository,
            ICompanyEmployeeRepository companyEmployeeRepository)
        {
            _jobRepository = jobRepository;
            _skillRepository = skillRepository;
            _companyEmployeeRepository = companyEmployeeRepository;
        }

        public async Task<ServiceResult<QueryResponse<JobDto>>> GetAll(JobQueryDto jobQueryDto)
        {
            var data = await _jobRepository.GetAllAsync(jobQueryDto);

            return ServiceResult<QueryResponse<JobDto>>.Success(new QueryResponse<JobDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(j => j.ToJobDto())
            });
        }

        public async Task<ServiceResult<Job>> FindById(int id)
        {
            var job = await _jobRepository.GetByIdAsync(id);

            if(job is null)
                return ServiceResult<Job>.Failure(StatusCodes.Status404NotFound, 
                                                                  Error.NotFound, 
                                                                  [ErrorMessages.JobNotFound]);
            return ServiceResult<Job>.Success(job);
        }

        public async Task<ServiceResult<Job>> Create(CreateJobDto createJobDto, ClaimsPrincipal user)
        {
            // Max salary must be greater than min salary
            if (createJobDto.SalaryMax <= createJobDto.SalaryMin)
                return ServiceResult<Job>.Failure(StatusCodes.Status400BadRequest, 
                                                                  Error.ViolatedRule, 
                                                                  [ErrorMessages.InvalidSalaryRange]);

            // End date must be after start date
            if (createJobDto.EndDate <= createJobDto.StartDate)
                return ServiceResult<Job>.Failure(StatusCodes.Status400BadRequest,
                                                                  Error.ViolatedRule,
                                                                  [ErrorMessages.InvalidDateRange]);

            // Start date and end date cannot be in the past
            if (createJobDto.StartDate < DateTimeOffset.UtcNow || createJobDto.EndDate < DateTimeOffset.UtcNow)
                return ServiceResult<Job>.Failure(StatusCodes.Status400BadRequest,
                                                                  Error.ViolatedRule,
                                                                  [ErrorMessages.InvalidDateValue]);

            // Duration from start to end must be at least 7 days
            if ((createJobDto.EndDate - createJobDto.StartDate).TotalDays < 7)
                return ServiceResult<Job>.Failure(StatusCodes.Status400BadRequest,
                                                                  Error.ViolatedRule,
                                                                  [ErrorMessages.InvalidJobDuration]);

            var userId = user.GetUserId()!;
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);

            // User must be in a company to create a job
            if (userEmployment is null)
                return ServiceResult<Job>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ActiveEmploymentNotFound]);
            
            var skills = await _skillRepository.FindAsync(createJobDto.SkillIds);

            var job = new Job
            {
                Name = createJobDto.Name,
                Location = createJobDto.Location,
                Quantity = createJobDto.Quantity,
                Level = createJobDto.Level,
                Description = createJobDto.Description,
                SalaryMin = createJobDto.SalaryMin,
                SalaryMax = createJobDto.SalaryMax,
                StartDate = createJobDto.StartDate,
                EndDate = createJobDto.EndDate,
                Status = JobStatus.Pending,
                CompanyEmployeeId = userEmployment.Id,
                CompanyId = userEmployment.CompanyId,
                Skills = [.. skills]
            };

            await _jobRepository.CreateAsync(job);

            return ServiceResult<Job>.Success(job);
        }

        public async Task<ServiceResult<Job>> UpdateStatus(int id, UpdateJobStatusDto updateJobStatusDto, ClaimsPrincipal user)
        {
            var userId = user.GetUserId()!;
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);

            if(updateJobStatusDto.Status == JobStatus.Pending)
                return ServiceResult<Job>.Failure(StatusCodes.Status400BadRequest,
                                                                  Error.ViolatedRule,
                                                                  [ErrorMessages.InvalidJobStatus]);

            // User must be in a company to update job status
            if (userEmployment is null)
                return ServiceResult<Job>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ActiveEmploymentNotFound]);

            // Only company owner can update job status
            if (userEmployment.CompanyRole != CompanyRole.Owner)
                return ServiceResult<Job>.Failure(StatusCodes.Status403Forbidden,
                                                                  Error.Forbidden,
                                                                  [ErrorMessages.NoPermission]);

            var job = await _jobRepository.GetByIdAsync(id);
            if (job is null)
                return ServiceResult<Job>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.JobNotFound]);

            // Can only update job status of jobs from their own company
            if (job.CompanyId != userEmployment.CompanyId)
                return ServiceResult<Job>.Failure(StatusCodes.Status403Forbidden,
                                                                  Error.Forbidden,
                                                                  [ErrorMessages.NoPermission]);

            // Cannot update status if the job is already closed
            if (job.Status == JobStatus.Closed)
                return ServiceResult<Job>.Failure(StatusCodes.Status409Conflict,
                                                                  Error.ResourceConflict,
                                                                  [ErrorMessages.JobClosed]);

            await _jobRepository.UpdateStatusAsync(job, updateJobStatusDto);

            return ServiceResult<Job>.Success(job);
        }

        public async Task<ServiceResult<Job>> Update(int id, UpdateJobDto updateJobDto, ClaimsPrincipal user)
        {
            var userId = user.GetUserId()!;
            var userEmployment = await _companyEmployeeRepository.GetActiveEmploymentAsync(userId);

            // User must be in a company to update job info
            if (userEmployment is null)
                return ServiceResult<Job>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ActiveEmploymentNotFound]);

            var job = await _jobRepository.GetByIdAsync(id);
            if (job is null)
                return ServiceResult<Job>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.JobNotFound]);

            // Can only update job created by themselves
            if (job.CompanyEmployeeId != userEmployment.Id)
                return ServiceResult<Job>.Failure(StatusCodes.Status403Forbidden,
                                                                  Error.Forbidden,
                                                                  [ErrorMessages.NoPermission]);

            // Cannot update job if it's already closed or approved
            if (job.Status != JobStatus.Pending)
                return ServiceResult<Job>.Failure(StatusCodes.Status409Conflict,
                                                                  Error.ResourceConflict,
                                                                  [ErrorMessages.JobUpdateNotAllowed]);

            var skills = await _skillRepository.FindAsync(updateJobDto.SkillIds);

            await _jobRepository.UpdateAsync(job, updateJobDto, skills);

            return ServiceResult<Job>.Success(job);
        }

    }
}
