using JobNexus.Dtos.Skill;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ISkillRepository
    {
        Task<Skill> CreateAsync(CreateSkillDto createSkillDto);
    }
}
