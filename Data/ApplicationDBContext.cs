using JobNexus.Helpers.Interceptors;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JobNexus.Data
{
    public class ApplicationDBContext : IdentityDbContext<AppUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new SoftDeleteInterceptor(), 
                                          new TimestampInterceptor());

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>().HasQueryFilter(x => x.IsDeleted == false);
            builder.Entity<CompanyEmployee>().HasQueryFilter(x => x.AppUser != null && x.AppUser.IsDeleted == false);
            builder.Entity<CompanyRequest>().HasQueryFilter(x => x.AppUser != null && x.AppUser.IsDeleted == false);
            builder.Entity<Skill>().HasQueryFilter(x => x.IsDeleted == false);
            builder.Entity<Job>().HasQueryFilter(x => x.IsDeleted == false);
            builder.Entity<Resume>().HasQueryFilter(x => x.IsDeleted == false && 
                                                         x.AppUser != null && x.AppUser.IsDeleted == false);
            builder.Entity<Application>().HasQueryFilter(x => x.AppUser != null && x.AppUser.IsDeleted == false && 
                                                              x.Job != null && x.Job.IsDeleted ==false);
            builder.Entity<Token>().HasQueryFilter(x => x.AppUser != null && x.AppUser.IsDeleted == false);

            builder.Entity<Application>()
                   .HasIndex(a => new { a.JobId, a.AppUserId })
                   .IsUnique();
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyEmployee> CompanyEmployees { get; set; }
        public DbSet<CompanyRequest> CompanyRequests { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<ResumeVersion> ResumeVersions { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Token> Tokens { get; set; }
    }

}
