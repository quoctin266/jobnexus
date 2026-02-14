using JobNexus.Dtos.Resume;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class ResumeMappers
    {
        public static ResumeDto ToResumeDto(this Resume resume)
        {
            return new ResumeDto
            {
                Id = resume.Id,
                Title = resume.Title,
                IsDefault = resume.IsDefault,
                FileUrl = resume.ResumeVersions.Count > 0 ? resume.ResumeVersions[0].FileUrl : "",
                CreatedBy = resume.AppUser?.ToUserSummaryDto(),
                CreatedAt = resume.CreatedAt,
                UpdatedAt = resume.UpdatedAt
            };
        }
    }
}
