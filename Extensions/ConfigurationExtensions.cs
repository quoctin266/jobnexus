using JobNexus.Helpers.Utils;

namespace JobNexus.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ACSSettings>(configuration.GetSection("ACS"));
            services.Configure<FrontendSettings>(configuration.GetSection("Frontend"));

            return services;
        }
    }
}
