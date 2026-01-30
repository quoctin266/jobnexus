using JobNexus.Common.Constant;
using JobNexus.Helpers.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace JobNexus.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddGlobalAuth(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });

            return services;
        }

        public static IServiceCollection AddAuthPolicy(this IServiceCollection services)
        {
            //services.AddAuthorizationBuilder()
            //        .AddPolicy(Policy.ResourceOwner, policy => policy.Requirements.Add(new ResourceOwnerRequirement()));

            return services;
        }

        public static IServiceCollection AddAuthHandler(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationHandler, CompanyRequestOwnerHandler>();

            return services;
        }
    }
}
