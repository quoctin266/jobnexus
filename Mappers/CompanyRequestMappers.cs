using JobNexus.Dtos.CompanyRequest;
using JobNexus.Models;

namespace JobNexus.Mappers
{
    public static class CompanyRequestMappers
    {
        public static CompanyRequestDto ToCompanyRequestDto(this CompanyRequest companyRequest)
        {
            return new CompanyRequestDto
            {
                Id = companyRequest.Id,

                Name = companyRequest.Name,

                Address = companyRequest.Address,

                Description = companyRequest.Description,

                TIN = companyRequest.TIN,

                BusinessLicenseUrl = companyRequest.BusinessLicenseUrl,

                EmploymentContractUrl = companyRequest.EmploymentContractUrl,

                Status = companyRequest.Status,

                CreatedAt = companyRequest.CreatedAt,

                CreatedBy = companyRequest.AppUser?.ToUserDto()
            };
        }
    }
}
