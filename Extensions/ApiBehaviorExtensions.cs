using JobNexus.Common.Constant;
using JobNexus.Helpers.Utils;
using Microsoft.AspNetCore.Mvc;

namespace JobNexus.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection AddCustomErrorResponse(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var messages = context.ModelState
                        .Where(e => e.Value!.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return new BadRequestObjectResult(new ApiErrorResponse(StatusCodes.Status400BadRequest, 
                                                                           messages, Error.ValidationFailed));
                };
            });

            return services;
        }

    }
}
