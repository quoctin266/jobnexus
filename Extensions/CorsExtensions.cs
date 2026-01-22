namespace JobNexus.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("https://jobnexus.com.vn")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            return services;
        }
    }
}
