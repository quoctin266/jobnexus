using JobNexus.Dtos.Job;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class JobMappers
    {
        public static JobDto ToJobDto(this Job job)
        {
            return new JobDto
            {
                Id = job.Id,
                Name = job.Name,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Quantity = job.Quantity,
                Level = job.Level,
                Description = job.Description,
                StartDate = job.StartDate,
                EndDate = job.EndDate,
                IsActive = job.IsActive,
                Company = job.Company?.ToCompanySummaryDto(),
                CreatedBy = job.CompanyEmployee?.ToCompanyEmployeeSummaryDto(),
                Skills = job.Skills.Select(s => s.ToSkillSummaryDto()).ToList(),
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt
            };
        }
    }
}
