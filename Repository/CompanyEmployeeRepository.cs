using JobNexus.Data;
using JobNexus.Dtos.CompanyEmployee;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;

namespace JobNexus.Repository
{
    public class CompanyEmployeeRepository : ICompanyEmployeeRepository
    {
        private readonly ApplicationDBContext _context;

        public CompanyEmployeeRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<CompanyEmployee?> GetActiveEmploymentAsync(string userId)
        {
            return await _context.CompanyEmployees.Include(ce => ce.Company)
                                                  .Include(ce => ce.AppUser)
                                                  .FirstOrDefaultAsync(ce => ce.AppUserId == userId && ce.IsActive == true);
        }

        public async Task<CompanyEmployee?> GetByIdAsync(int CompanyEmployeeId)
        {
            return await _context.CompanyEmployees.Include(ce => ce.Company)
                                                  .Include(ce => ce.AppUser)
                                                  .FirstOrDefaultAsync(ce => ce.Id == CompanyEmployeeId);
        }

        public async Task<CompanyEmployee> CreateAsync(CreateCompanyEmployeeDto createCompanyEmployeeDto)
        {
            var companyEmployee = new CompanyEmployee
            {
                CompanyId = createCompanyEmployeeDto.CompanyId,
                AppUserId = createCompanyEmployeeDto.AppUserId,
                EmploymentContractUrl = createCompanyEmployeeDto.EmploymentContractUrl,
                CompanyRole = createCompanyEmployeeDto.CompanyRole,
            };

            await _context.CompanyEmployees.AddAsync(companyEmployee);
            await _context.SaveChangesAsync();

            return companyEmployee;
        }
    }
}
