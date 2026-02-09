using JobNexus.Dtos.Skill;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ISkillService
    {
        Task<ServiceResult<QueryResponse<SkillDto>>> GetAllAsync(SkillQueryDto skillQueryDto);

        Task<ServiceResult<Skill>> CreateAsync(CreateSkillDto createSkillDto);

        Task<ServiceResult<Skill>> UpdateAsync(int id, UpdateSkillDto updateSkillDto);

        Task<ServiceResult<VoidType>> DeleteAsync(int id);
    }
}
