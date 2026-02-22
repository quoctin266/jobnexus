using JobNexus.Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Application
{
    public record UpdateApplicationStatusDto
    {
        [Required]
        public ApplicationStatus Status { get; set; }
    }
}
