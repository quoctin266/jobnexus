using JobNexus.Common.Enum;
using JobNexus.Helpers.Utils;

namespace JobNexus.Dtos.CompanyEmployee
{
    public record CompanyEmployeeQueryDto : BaseQueryDto
    {
        public CompanyRole? Role { get; init; }

        public bool? IsActive { get; init; }

        public int? CompanyId { get; init; }

        public string? UserId { get; init; }
    }
}
