using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ICompanyRepository
    {
        Task<Company> CreateAsync(CompanyRequest companyRequest);

        Task<Company?> GetByIdAsync(int companyId);
    }
}
