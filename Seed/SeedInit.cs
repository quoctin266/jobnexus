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

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded && role == "Admin")
                    {
                        await CreateAdminAsync(userManager, role, configuration);
                    }
                }
                else if (role == "Admin")
                {
                    await CreateAdminAsync(userManager, role, configuration);
                }
            }
        }

        public static async Task CreateAdminAsync(UserManager<AppUser> userManager, string role, IConfiguration configuration)
        {
            var username = configuration["Admin:Username"]!;
            var email = configuration["Admin:Email"]!;
            var password = configuration["Admin:Password"]!;

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var admin = new AppUser
                {
                    UserName = username,
                    Email = email
                };

                var result = await userManager.CreateAsync(admin, password);

                if (result.Succeeded) await userManager.AddToRoleAsync(admin, role);
            }

        }
    }
}
