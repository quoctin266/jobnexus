using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Skill
{
    public record UpdateSkillDto
    {
        [Required]
        public bool IsActive { get; init; }
    }
}
