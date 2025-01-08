using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<LibraryContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure the database is created
            context.Database.EnsureCreated();

            // Check if there are any users
            if (!context.Users.Any())
            {
                // Create roles if they don't exist
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                if (!await roleManager.RoleExistsAsync("User"))
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                // Create admin user
                var adminUser = new ApplicationUser
                {
                    UserName = "admin@library.com",
                    Email = "admin@library.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // Add other seed data if needed
            if (!context.Authors.Any())
            {
                // Add sample authors
                context.Authors.AddRange(
                    new Author { Name = "George Orwell" },
                    new Author { Name = "J.K. Rowling" }
                );
                await context.SaveChangesAsync();
            }

            // Add sample books if none exist
            if (!context.Books.Any())
            {
                var author = await context.Authors.FirstOrDefaultAsync();
                if (author != null)
                {
                    context.Books.AddRange(
                        new Book { Title = "1984", ISBN = "978-0451524935", AuthorId = author.AuthorId },
                        new Book { Title = "Animal Farm", ISBN = "978-0451526342", AuthorId = author.AuthorId }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 