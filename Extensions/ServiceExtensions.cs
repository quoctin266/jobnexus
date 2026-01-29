using JobNexus.Interfaces;
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
            services.AddScoped<ICompanyRequestService, CompanyRequestService>();

            return services;
        }

    }
}
