using JobNexus.Dtos.Application;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class ApplicationMappers
    {
        public static ApplicationDto ToApplicationDto(this Application application)
        {
            return new ApplicationDto
            {
                Id = application.Id,
                FullName = application.FullName,
                PhoneNumber = application.PhoneNumber,
                Email = application.Email,
                Intro = application.Intro,
                ResumeUrl = application.ResumeVersion?.FileUrl ?? "",
                Status = application.Status,
                Job = application.Job?.ToJobDto(),
                CreatedBy = application.AppUser?.ToUserSummaryDto(),
                CreatedAt = application.CreatedAt,
                UpdatedAt = application.UpdatedAt
            };
        }
    }
}
