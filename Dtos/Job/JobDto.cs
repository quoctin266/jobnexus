using JobNexus.Common.Enum;
using JobNexus.Dtos.Company;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Dtos.Skill;

namespace JobNexus.Dtos.Job
{
    public record JobDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public string Location { get; set; } = "";

        public decimal SalaryMin { get; set; }

        public decimal SalaryMax { get; set; }

        public int Quantity { get; set; }

        public string Level { get; set; } = "";

        public string Description { get; set; } = "";

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public JobStatus Status { get; set; }

        public CompanySummaryDto? Company { get; set; }

        public CompanyEmployeeSummaryDto? CreatedBy { get; set; }

        public List<SkillSummaryDto> Skills { get; set; } = [];

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
