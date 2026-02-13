using JobNexus.Dtos.Skill;
using JobNexus.Helpers.Utils;
using JobNexus.Models;

namespace JobNexus.Interfaces.BusinessService
{
    public interface ISkillService
    {
        Task<ServiceResult<QueryResponse<SkillDto>>> GetAll(SkillQueryDto skillQueryDto);

        Task<ServiceResult<Skill>> Create(CreateSkillDto createSkillDto);

        Task<ServiceResult<Skill>> Update(int id, UpdateSkillDto updateSkillDto);

        Task<ServiceResult<VoidType>> Delete(int id);
    }
}
