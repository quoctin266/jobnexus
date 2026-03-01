using JobNexus.Common.Constant.Messages;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Skill
{
    public record CreateSkillDto()
    {
        [Required]
        [MaxLength(50, ErrorMessage = ValidationMessages.SkillNameMaxLength)]
        public string Name { get; init; } = "";
    }
}
