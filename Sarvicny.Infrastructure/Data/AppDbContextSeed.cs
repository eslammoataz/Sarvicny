using Microsoft.AspNetCore.Identity;
using Sarvicny.Domain.Entities.Users;


namespace Sarvicny.Infrastructure.Data
{
    public static class AppDbContextSeed
    {
        public static async Task SeedData(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            await SeedAdmin(userManager, context);
            await SeedRoles(roleManager);
        }

        private static async Task SeedAdmin(UserManager<User> userManager, AppDbContext context)
        {

            if (!context.Admins.Any())
            {
                Admin admin = new Admin
                {
                    Id = "1",
                    UserName = "admin@example.com",
                    NormalizedUserName = "ADMIN@EXAMPLE.COM",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                };

                await userManager.CreateAsync(admin, "Admin123#");

            }

        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var admin = new IdentityRole("Admin");
                await roleManager.CreateAsync(admin);
            }

            if (!await roleManager.RoleExistsAsync("Customer"))
            {
                var customer = new IdentityRole("Customer");
                await roleManager.CreateAsync(customer);
            }

            if (!await roleManager.RoleExistsAsync("ServiceProvider"))
            {
                var worker = new IdentityRole("ServiceProvider");
                await roleManager.CreateAsync(worker);
            }
        }
    }
}
