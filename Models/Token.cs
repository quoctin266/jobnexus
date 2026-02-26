using JobNexus.Common.Enum;
using JobNexus.Interfaces;

namespace JobNexus.Models
{
    public class Token : IEntityTimestamps
    {
        public int Id { get; set; }

        public Guid TokenIdentity { get; set; }

        public TokenPurpose Purpose { get; set; }

        public string AppUserId { get; set; } = "";

        public AppUser? AppUser { get; set; }

        public DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public DateTimeOffset UpdatedAt { get; set; }
    }
}
