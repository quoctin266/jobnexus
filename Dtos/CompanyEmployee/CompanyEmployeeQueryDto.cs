using JobNexus.Common.Constant.Messages;
using JobNexus.Common.Enum;
using JobNexus.Helpers.Attributes;
using JobNexus.Helpers.Utils;

namespace JobNexus.Dtos.CompanyEmployee
{
    public record CompanyEmployeeQueryDto : BaseQueryDto
    {
        [ValidEnum(typeof(CompanyRole), ErrorMessage = ValidationMessages.CompanyRoleValue)]
        public CompanyRole? Role { get; init; }

        public bool? IsActive { get; init; }

        public int? CompanyId { get; init; }

        public string? UserId { get; init; }
    }
}
