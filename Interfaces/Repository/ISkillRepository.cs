using JobNexus.Dtos.Skill;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ISkillRepository
    {
        Task<QueryResponse<Skill>> GetAllAsync(SkillQueryDto skillQueryDto);

        Task<Skill?> GetByIdAsync(int id);

        Task<bool> IsInUse(Skill skill);

        Task<Skill> CreateAsync(CreateSkillDto createSkillDto);

        Task<Skill> UpdateAsync(Skill skill, UpdateSkillDto updateSkillDto);

        Task DeleteAsync(Skill skill);
    }
}
