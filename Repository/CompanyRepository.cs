using JobNexus.Data;
using JobNexus.Dtos.Company;
using JobNexus.Extensions;
using JobNexus.Helpers.Utils;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobNexus.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDBContext _context;

        private readonly Dictionary<string, Expression<Func<Company, object>>> _sortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = cr => cr.Name,
            ["CreatedAt"] = cr => cr.CreatedAt,
        };

        public CompanyRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<QueryResponse<Company>> GetAllAsync(CompanyQueryDto companyQueryDto)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrWhiteSpace(companyQueryDto.Name))
            {
                query = query.Where(c => c.Name.ToLower().Contains(companyQueryDto.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(companyQueryDto.TIN))
            {
                query = query.Where(c => c.TIN.Contains(companyQueryDto.TIN));
            }

            if (companyQueryDto.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == companyQueryDto.IsActive);
            }

            if (!string.IsNullOrWhiteSpace(companyQueryDto.SortBy))
            {
                query = query.ApplySorting(companyQueryDto.SortBy, companyQueryDto.IsDescending, _sortMap);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)companyQueryDto.PageSize);

            var offset = (companyQueryDto.PageNumber - 1) * companyQueryDto.PageSize;
            var items = await query.Skip(offset).Take(companyQueryDto.PageSize).ToListAsync();

            return new QueryResponse<Company>
            {
                TotalPages = totalPages,
                PageNumber = companyQueryDto.PageNumber,
                PageSize = companyQueryDto.PageSize,
                TotalItems = totalItems,
                Items = items
            };
        }

        public async Task<Company?> GetByIdAsync(int companyId)
        {
            return await _context.Companies.FindAsync(companyId);
        }

        public async Task<Company> CreateAsync(CompanyRequest companyRequest)
        {
            var company = new Company()
            {
                Name = companyRequest.Name,
                Address = companyRequest.Address,
                Description = companyRequest.Description,
                TIN = companyRequest.TIN,
                BusinessLicenseUrl = companyRequest.BusinessLicenseUrl,
            };

            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<Company> UpdateAsync(Company company, UpdateCompanyDto updateCompanyDto)
        {
            company.Address = updateCompanyDto.Address;
            company.Description = updateCompanyDto.Description;

            await _context.SaveChangesAsync();

            return company;
        }

        public async Task<Company> UpdateStatusAsync(Company company, bool IsActive)
        {
            company.IsActive = IsActive;

            await _context.SaveChangesAsync();

            return company;
        }
    }
}
