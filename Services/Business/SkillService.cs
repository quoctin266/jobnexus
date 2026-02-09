using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Dtos.CompanyRequest;
using JobNexus.Dtos.Skill;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Interfaces.Repository;
using JobNexus.Mappers;
using JobNexus.Models;
using JobNexus.Repository;

namespace JobNexus.Services.Business
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;

        public SkillService(ISkillRepository skillRepository)
        {
            _skillRepository = skillRepository;
        }

        public async Task<ServiceResult<QueryResponse<SkillDto>>> GetAllAsync(SkillQueryDto skillQueryDto)
        {
            var data = await _skillRepository.GetAllAsync(skillQueryDto);

            return ServiceResult<QueryResponse<SkillDto>>.Success(new QueryResponse<SkillDto>
            {
                TotalPages = data.TotalPages,
                PageNumber = data.PageNumber,
                PageSize = data.PageSize,
                TotalItems = data.TotalItems,
                Items = data.Items.Select(sk => sk.ToSkillDto())
            });
        }

        public async Task<ServiceResult<Skill>> CreateAsync(CreateSkillDto createSkillDto)
        {
            var skill = await _skillRepository.CreateAsync(createSkillDto);

            return ServiceResult<Skill>.Success(skill);
        }

        public async Task<ServiceResult<Skill>> UpdateAsync(int id, UpdateSkillDto updateSkillDto)
        {
            var skill = await _skillRepository.GetByIdAsync(id);

            if (skill is null)
                return ServiceResult<Skill>.Failure(StatusCodes.Status404NotFound, 
                                                    Error.NotFound, [ErrorMessages.SkillNotFound]);
             
            await _skillRepository.UpdateAsync(skill, updateSkillDto);

            return ServiceResult<Skill>.Success(skill);
        }

        public async Task<ServiceResult<VoidType>> DeleteAsync(int id)
        {
            var skill = await _skillRepository.GetByIdAsync(id);

            if(skill is null)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status404NotFound, 
                                                    Error.NotFound, [ErrorMessages.SkillNotFound]);

            var isInUse = await _skillRepository.IsInUse(skill);

            if(isInUse)
                return ServiceResult<VoidType>.Failure(StatusCodes.Status409Conflict, 
                                                    Error.ResourceConflict, 
                                                    [ErrorMessages.SkillInUse]);

            await _skillRepository.DeleteAsync(skill);

            return ServiceResult<VoidType>.Success(new VoidType());
        }
    }
}
