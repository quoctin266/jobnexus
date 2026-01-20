using JobNexus.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Models
{
    public class AppUser : IdentityUser, IEntityTimestamps, ISoftDelete
    {
        public int Age { get; set; }

        public string Address { get; set; } = "";

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }

        public bool IsDeleted { get; set; } 

        public DateTimeOffset? DeletedAt { get; set; }
    }
}
