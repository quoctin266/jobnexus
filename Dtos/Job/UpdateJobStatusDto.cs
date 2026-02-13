using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Job
{
    public record UpdateJobStatusDto
    {
        [Required]
        public bool IsActive { get; init; }
    }
}
