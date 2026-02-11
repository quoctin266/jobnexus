using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Job
{
    public record CreateJobDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.JobNameMaxLength)]
        public string Name { get; set; } = "";

        [Required]
        [MaxLength(20, ErrorMessage = ValidationMessages.JobNameMaxLength)]
        public string Location { get; set; } = "";

        [Required]
        [Range(1_000_000, 100_000_000, ErrorMessage = ValidationMessages.JobSalaryRange)]
        public decimal SalaryMin { get; set; }

        [Required]
        [Range(1_000_000, 100_000_000, ErrorMessage = ValidationMessages.JobSalaryRange)]
        public decimal SalaryMax { get; set; }

        [Required]
        [Range(1, 50, ErrorMessage = ValidationMessages.JobQuantityRange)]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = ValidationMessages.JobLevelMaxLength)]
        public string Level { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public DateTimeOffset StartDate { get; set; }

        [Required]
        [Iso8601Date(ErrorMessage = ValidationMessages.DoBFormat)]
        public DateTimeOffset EndDate { get; set; }

        [Required]
        [Length(1, 10, ErrorMessage = ValidationMessages.JobSkillRange)]
        public List<int> SkillIds { get; set; } = [];
    }
}
