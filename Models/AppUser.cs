using JobNexus.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Models
{
    public class AppUser : IdentityUser, IEntityTimestamps
    {
        public int Age { get; set; }

        public string Address { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
