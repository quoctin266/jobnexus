using JobNexus.Interfaces.Repository;
using JobNexus.Repository;

namespace JobNexus.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICompanyRequestRepository, CompanyRequestRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyEmployeeRepository, CompanyEmployeeRepository>();
            services.AddScoped<ISkillRepository, SkillRepository>();
            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IResumeRepository, ResumeRepository>();
            services.AddScoped<IResumeVersionRepository, ResumeVersionRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();

            return services;
        }

    }
}
