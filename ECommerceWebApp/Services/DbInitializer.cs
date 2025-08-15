using Microsoft.AspNetCore.Identity;
using ECommerceWebApp.Data;
using ECommerceWebApp.Models;

namespace ECommerceWebApp.Services
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Create roles
            string[] roles = { "Admin", "Seller", "Buyer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Create admin user
            var adminEmail = "admin@ecommerce.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            
            if (adminUser == null)
            {
                adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Create sample seller
            var sellerEmail = "seller@ecommerce.com";
            var sellerUser = await userManager.FindByEmailAsync(sellerEmail);
            
            if (sellerUser == null)
            {
                sellerUser = new User
                {
                    UserName = sellerEmail,
                    Email = sellerEmail,
                    FirstName = "Sample",
                    LastName = "Seller",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(sellerUser, "Seller123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(sellerUser, "Seller");
                }
            }

            // Create sample buyer
            var buyerEmail = "buyer@ecommerce.com";
            var buyerUser = await userManager.FindByEmailAsync(buyerEmail);
            
            if (buyerUser == null)
            {
                buyerUser = new User
                {
                    UserName = buyerEmail,
                    Email = buyerEmail,
                    FirstName = "Sample",
                    LastName = "Buyer",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(buyerUser, "Buyer123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(buyerUser, "Buyer");
                }
            }
        }
    }
}
