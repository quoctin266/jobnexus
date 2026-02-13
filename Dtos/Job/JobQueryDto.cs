using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Job
{
    public record JobQueryDto : BaseQueryDto
    {
        [MaxLength(50, ErrorMessage = ValidationMessages.JobNameMaxLength)]
        public string? Name { get; init; }

        [MaxLength(20, ErrorMessage = ValidationMessages.JobLocationMaxLength)]
        public string? Location { get; init; }

        [Range(1_000_000, 100_000_000, ErrorMessage = ValidationMessages.JobSalaryRange)]
        public decimal? SalaryMin { get; init; }

        [Range(1_000_000, 100_000_000, ErrorMessage = ValidationMessages.JobSalaryRange)]
        public decimal? SalaryMax { get; init; }

        [Range(1, 50, ErrorMessage = ValidationMessages.JobQuantityRange)]
        public int? Quantity { get; init; }

        [MaxLength(20, ErrorMessage = ValidationMessages.JobLevelMaxLength)]
        public string? Level { get; init; } = "";

        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public DateTimeOffset? StartDate { get; init; }

        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public DateTimeOffset? EndDate { get; init; }

        [Length(1, 10, ErrorMessage = ValidationMessages.JobSkillRange)]
        public List<int>? SkillIds { get; init; }

        public int? CompanyId { get; init; }

        public int? CompanyEmployeeId { get; init; }

        public bool? IsActive { get; init; }
    }
}
