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
            builder.Entity<Company>().HasQueryFilter(x => x.IsDeleted == false);
            builder.Entity<CompanyEmployee>().HasQueryFilter(x => x.AppUser != null && x.AppUser.IsDeleted == false);
            builder.Entity<CompanyRequest>().HasQueryFilter(x => x.AppUser != null && x.AppUser.IsDeleted == false);
        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyEmployee> CompanyEmployees { get; set; }
        public DbSet<CompanyRequest> CompanyRequests { get; set; }
    }

}
