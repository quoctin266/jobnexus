using JobNexus.Dtos.Resume;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.BusinessService
{
    public interface IResumeService
    {
        Task<ServiceResult<QueryResponse<ResumeDto>>> GetAll(ResumeQueryDto resumeQueryDto, ClaimsPrincipal user);

        Task<ServiceResult<Resume>> FindById(int id, ClaimsPrincipal user);

        Task<ServiceResult<Resume>> Create(CreateResumeDto createResumeDto, ClaimsPrincipal user);

        Task<ServiceResult<Resume>> Update(int id, UpdateResumeDto updateResumeDto, ClaimsPrincipal user);

        Task<ServiceResult<VoidType>> Delete(int id, ClaimsPrincipal user);
    }
}
