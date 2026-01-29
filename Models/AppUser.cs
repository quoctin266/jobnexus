using JobNexus.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Models
{
    public class AppUser : IdentityUser, IEntityTimestamps, ISoftDelete
    {
        public DateTime DateOfBirth { get; set; }

        public string Gender { get; set; } = "";

        public string Address { get; set; } = "";

        public List<CompanyEmployee> CompanyEmployees { get; set; } = [];

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } 

        public DateTimeOffset? DeletedAt { get; set; }
    }
}
