using JobNexus.Common.Enum;
using JobNexus.Models;
using Microsoft.AspNetCore.Identity;

namespace JobNexus.Seed
{
    public static class SeedInit
    {
        // Seed roles and admin user
        public static async Task Initialize(this IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var roles = new[] { Role.Admin, Role.User, Role.Employer };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.ToString()))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role.ToString()));
                    if (result.Succeeded && role == Role.Admin)
                    {
                        await CreateAdminAsync(userManager, configuration);
                    }
                }
                else if (role == Role.Admin)
                {
                    await CreateAdminAsync(userManager, configuration);
                }
            }
        }

        public static async Task CreateAdminAsync(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            var username = configuration["Admin:Username"]!;
            var email = configuration["Admin:Email"]!;
            var password = configuration["Admin:Password"]!;

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var admin = new AppUser
                {
                    UserName = username,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded) await userManager.AddToRoleAsync(admin, Role.Admin.ToString());
            }

        }
    }
}
