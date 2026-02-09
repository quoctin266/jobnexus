using JobNexus.Data;
using JobNexus.Dtos.Skill;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobNexus.Repository
{
    public class SkillRepository : ISkillRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<Skill, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = cr => cr.Name,
            ["CreatedAt"] = cr => cr.CreatedAt
        };

        public SkillRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse<Skill>> GetAllAsync(SkillQueryDto skillQueryDto)
        {
            var query = _context.Skills.AsQueryable();

            if (!string.IsNullOrWhiteSpace(skillQueryDto.Name))
            {
                query = query.Where(sk => sk.Name.ToLower().Contains(skillQueryDto.Name.ToLower()));
            }

            if (skillQueryDto.IsActive.HasValue)
            {
                query = query.Where(sk => sk.IsActive == skillQueryDto.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(skillQueryDto.SortBy))
            {
                query = query.ApplySorting(skillQueryDto.SortBy, skillQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)skillQueryDto.PageSize);

            var offset = (skillQueryDto.PageNumber - 1) * skillQueryDto.PageSize;
            var items = await query.Skip(offset).Take(skillQueryDto.PageSize).ToListAsync();

            return new QueryResponse<Skill>
            {
                TotalPages = totalPages,
                PageNumber = skillQueryDto.PageNumber,
                PageSize = skillQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<Skill?> GetByIdAsync(int id)
        {
            return await _context.Skills.FindAsync(id);
        }

        public async Task<bool> IsInUse(Skill skill)
        {
            await _context.Entry(skill).Collection(sk => sk.Jobs).LoadAsync();

            return skill.Jobs.Count != 0;
        }

        public async Task<Skill> CreateAsync(CreateSkillDto createSkillDto)
        {
            var skill = new Skill
            {
                Name = createSkillDto.Name,
            };

            await _context.Skills.AddAsync(skill);
            await _context.SaveChangesAsync();

            return skill;
        }

        public async Task DeleteAsync(Skill skill)
        {
            _context.Skills.Remove(skill);

            await _context.SaveChangesAsync();
        }

        public async Task<Skill> UpdateAsync(Skill skill, UpdateSkillDto updateSkillDto)
        {
            skill.IsActive = updateSkillDto.IsActive;

            await _context.SaveChangesAsync();

            return skill;
        }
    }
}
