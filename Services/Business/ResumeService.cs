using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Data;
using JobNexus.Dtos.Resume;
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
    public class ResumeService : IResumeService
    {
        private readonly ApplicationDBContext _context;

        private readonly IBlobStorageService _blobStorageService;

        private readonly IResumeRepository _resumeRepository;

        private readonly IResumeVersionRepository _resumeVersionRepository;

        public ResumeService(ApplicationDBContext context, IBlobStorageService blobStorageService,
                             IResumeRepository resumeRepository, IResumeVersionRepository resumeVersionRepository)
        {
            _context = context;
            _resumeRepository = resumeRepository;
            _resumeVersionRepository = resumeVersionRepository;
            _blobStorageService = blobStorageService;
        }

        public async Task<ServiceResult<Resume>> FindById(int id, ClaimsPrincipal user)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);

            if (resume is null)
                return ServiceResult<Resume>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ResumeNotFound]);

            var userId = user.GetUserId();
            // Only admin or the owner of the resume can access it
            if (!user.IsInRole(Role.Admin.ToString()) && resume.AppUserId != userId)
                return ServiceResult<Resume>.Failure(StatusCodes.Status403Forbidden,
                                                                  Error.Forbidden,
                                                                  [ErrorMessages.NoPermission]);

            return ServiceResult<Resume>.Success(resume);
        }

        public async Task<ServiceResult<QueryResponse<ResumeDto>>> GetAll(ResumeQueryDto resumeQueryDto, ClaimsPrincipal user)
        {
            var data = await _resumeRepository.GetAllAsync(resumeQueryDto, user);

            return ServiceResult<QueryResponse<ResumeDto>>.Success(new QueryResponse<ResumeDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(j => j.ToResumeDto())
            });
        }

        public async Task<ServiceResult<Resume>> Create(CreateResumeDto createResumeDto, ClaimsPrincipal user)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var resumeFileTask = _blobStorageService.UploadFileAsync(createResumeDto.ResumeFile);

                var userId = user.GetUserId();
                var resume = await _resumeRepository.CreateAsync(createResumeDto, userId!);

                // If the created resume is set as default, update all other resumes of the user to non-default
                if (createResumeDto.IsDefault)
                    await _resumeRepository.UpdateDefaultAsync(resume.Id);

                var resumeVersion = await _resumeVersionRepository.CreateAsync(new ResumeVersion
                {
                    ResumeId = resume.Id,
                    FileUrl = await resumeFileTask
                });

                resume.ResumeVersions = [resumeVersion];

                await transaction.CommitAsync();

                return ServiceResult<Resume>.Success(resume);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                await transaction.RollbackAsync();
            }

            return ServiceResult<Resume>.Failure(StatusCodes.Status500InternalServerError,
                                                         Error.ServerFailure,
                                                         [ErrorMessages.ServerError]);
        }

        public async Task<ServiceResult<Resume>> Update(int id, UpdateResumeDto updateResumeDto, ClaimsPrincipal user)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);

            if (resume is null)
                return ServiceResult<Resume>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ResumeNotFound]);

            var userId = user.GetUserId();
            // Only the owner of the resume can update it
            if (resume.AppUserId != userId)
                return ServiceResult<Resume>.Failure(StatusCodes.Status403Forbidden,
                                                                  Error.Forbidden,
                                                                  [ErrorMessages.NoPermission]);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _resumeRepository.UpdateAsync(resume, updateResumeDto);

                // If the updated resume is set as default, update all other resumes of the user to non-default
                if (updateResumeDto.IsDefault)
                    await _resumeRepository.UpdateDefaultAsync(resume.Id);

                if (updateResumeDto.ResumeFile is not null)
                {
                    var fileUrl = await _blobStorageService.UploadFileAsync(updateResumeDto.ResumeFile);

                    var newVersion = await _resumeVersionRepository.CreateAsync(new ResumeVersion
                    {
                        ResumeId = resume.Id,
                        FileUrl = fileUrl
                    });

                    resume.ResumeVersions = [newVersion];
                }

                await transaction.CommitAsync();

                return ServiceResult<Resume>.Success(resume);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                await transaction.RollbackAsync();
            }

            return ServiceResult<Resume>.Failure(StatusCodes.Status500InternalServerError,
                                                         Error.ServerFailure,
                                                         [ErrorMessages.ServerError]);
        }

        public async Task<ServiceResult<VoidType>> Delete(int id, ClaimsPrincipal user)
        {
            var resume = await _resumeRepository.GetByIdAsync(id);

            if (resume is null)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status404NotFound,
                                                                  Error.NotFound,
                                                                  [ErrorMessages.ResumeNotFound]);

            var userId = user.GetUserId();
            // Only the owner of the resume can delete it
            if (resume.AppUserId != userId)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status403Forbidden,
                                                                  Error.Forbidden,
                                                                  [ErrorMessages.NoPermission]);

            await _resumeRepository.DeleteAsync(resume);

            return ServiceResult<VoidType>.Success(new VoidType());
        }
    }
}
