using JobNexus.Dtos.Skill;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class SkillMappers
    {
        public static SkillDto ToSkillDto(this Skill skill)
        {
            return new SkillDto
            {
                Id = skill.Id,
                Name = skill.Name,
                IsActive = skill.IsActive,
                CreatedAt = skill.CreatedAt,
                UpdatedAt = skill.UpdatedAt
            };
        }

        public static SkillSummaryDto ToSkillSummaryDto(this Skill skill)
        {
            return new SkillSummaryDto
            {
                Id = skill.Id,
                Name = skill.Name,
            };
        }
    }
}
