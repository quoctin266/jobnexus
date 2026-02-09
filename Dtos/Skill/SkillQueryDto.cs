using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Utils;
using System.ComponentModel.DataAnnotations;

namespace JobNexus.Dtos.Skill
{
    public record SkillQueryDto : BaseQueryDto
    {
        [MaxLength(20, ErrorMessage = ValidationMessages.SkillNameMaxLength)]
        public string? Name { get; init; }

        public bool? IsActive { get; init; }
    }
}
