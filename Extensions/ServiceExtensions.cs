using JobNexus.Interfaces;
using JobNexus.Services;

namespace JobNexus.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }

    }
}
