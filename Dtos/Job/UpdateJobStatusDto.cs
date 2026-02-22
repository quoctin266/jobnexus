using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Job
{
    public record UpdateJobStatusDto
    {
        [Required]
        public JobStatus Status { get; init; }
    }
}
