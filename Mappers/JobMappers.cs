using JobNexus.Dtos.Job;
using JobNexus.Dtos.Skill;
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
                Company = job.Company?.ToCompanyDto(),
                CreatedBy = job.CompanyEmployee?.ToCompanyEmployeeDto(),
                Skills = job.Skills.Select(s => s.ToSkillDto()).ToList(),
                CreatedAt = job.CreatedAt,
                UpdatedAt = job.UpdatedAt
            };
        }
    }
}
