using JobNexus.Interfaces;
using JobNexus.Interfaces.BusinessService;
using JobNexus.Services;
using JobNexus.Services.Business;

namespace JobNexus.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddSingleton<IBlobStorageService, BlobStorageService>();

            // business services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICompanyRequestService, CompanyRequestService>();
            services.AddScoped<ICompanyEmployeeService, CompanyEmployeeService>();
            services.AddScoped<ISkillService, SkillService>();
            services.AddScoped<IJobService, JobService>();
            services.AddScoped<IResumeService, ResumeService>();
            services.AddScoped<ICompanyService, CompanyService>();

            return services;
        }

    }
}
