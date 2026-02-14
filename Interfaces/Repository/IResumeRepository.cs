using JobNexus.Dtos.Resume;
using JobNexus.Helpers.Utils;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces.Repository
{
    public interface IResumeRepository
    {
        Task<Resume?> GetByIdAsync(int id);

        Task<QueryResponse<Resume>> GetAllAsync(ResumeQueryDto resumeQueryDto, ClaimsPrincipal user);

        Task<Resume> CreateAsync(CreateResumeDto createResumeDto, string userId);

        Task<Resume> UpdateAsync(Resume resume, UpdateResumeDto updateResumeDto);

        Task UpdateDefaultAsync(int defaultResumeId);

        Task DeleteAsync(Resume resume);
    }
}
