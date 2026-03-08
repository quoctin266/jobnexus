using JobNexus.Common.Constant;
using JobNexus.Common.Constant.Messages;
using JobNexus.Helpers.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace JobNexus.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                options.DefaultChallengeScheme =
                options.DefaultForbidScheme =
                options.DefaultScheme =
                options.DefaultSignInScheme =
                options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
                // Use Identity's external sign-in cookie
                options.SignInScheme = IdentityConstants.ExternalScheme;
                options.Scope.Add("profile");
                options.Scope.Add("email");

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:SigningKey"]!))
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                       
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        var response = new ApiErrorResponse(StatusCodes.Status401Unauthorized, 
                                                           [ErrorMessages.InvalidToken], Error.UnAuthorized);

                        return context.Response.WriteAsJsonAsync(response);
                    },
                    
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        var response = new ApiErrorResponse(StatusCodes.Status403Forbidden, 
                                                           [ErrorMessages.NoPermission], Error.Forbidden);

                        return context.Response.WriteAsJsonAsync(response);
                    }
                };
            });

            return services;
        }
    }
}
