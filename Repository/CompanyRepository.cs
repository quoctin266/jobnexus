using JobNexus.Data;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;

namespace JobNexus.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDBContext _context;

        public CompanyRepository(ApplicationDBContext context)
        {
            _context = context;
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
    }
}
