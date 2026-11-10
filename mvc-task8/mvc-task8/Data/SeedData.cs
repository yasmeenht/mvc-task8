using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using mvc_task8.Models;

namespace mvc_task8.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider services, IConfiguration configuration)
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var role in new[] { "User", "Admin", "SuperAdmin" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var email = configuration["SuperAdmin:Email"];
            var password = configuration["SuperAdmin:Password"];
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                return;
            }

            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = "Super Admin"
                };
                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(string.Join(" ", result.Errors.Select(e => e.Description)));
                }
                await userManager.AddToRoleAsync(user, "SuperAdmin");
            }
        }
    }
}
