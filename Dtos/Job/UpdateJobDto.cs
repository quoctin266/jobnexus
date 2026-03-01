using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Job
{
    public record UpdateJobDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.JobNameMaxLength)]
        public string Name { get; init; } = "";

        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.JobNameMaxLength)]
        public string Location { get; init; } = "";

        [Required]
        [Range(1_000_000, 100_000_000, ErrorMessage = ValidationMessages.JobSalaryRange)]
        public decimal SalaryMin { get; init; }

        [Required]
        [Range(1_000_000, 100_000_000, ErrorMessage = ValidationMessages.JobSalaryRange)]
        public decimal SalaryMax { get; init; }

        [Required]
        [Range(1, 50, ErrorMessage = ValidationMessages.JobQuantityRange)]
        public int Quantity { get; init; }

        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.JobLevelMaxLength)]
        public string Level { get; init; } = "";

        [Required]
        public string Description { get; init; } = "";

        [Required]
        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public DateTimeOffset StartDate { get; init; }

        [Required]
        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public DateTimeOffset EndDate { get; init; }

        [Required]
        [Length(1, 10, ErrorMessage = ValidationMessages.JobSkillRange)]
        public List<int> SkillIds { get; init; } = [];
    }
}
