using JobNexus.Interfaces;
using JobNexus.Repository;

namespace JobNexus.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();

            return services;
        }

    }
}
