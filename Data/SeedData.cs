using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;

namespace LibraryApp.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new LibraryContext(
                serviceProvider.GetRequiredService<DbContextOptions<LibraryContext>>()))
            {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Ensure Admin role exists
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Create admin user if it doesn't exist
                var adminEmail = "admin@library.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var admin = new IdentityUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(admin, "Admin123!");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }

                if (!context.Authors.Any())
                {
                    var authors = new[]
                    {
                        new Author { Name = "J.K. Rowling", Books = new List<Book>() },
                        new Author { Name = "George R.R. Martin", Books = new List<Book>() },
                        new Author { Name = "Stephen King", Books = new List<Book>() },
                        new Author { Name = "J.R.R. Tolkien", Books = new List<Book>() }
                    };

                    context.Authors.AddRange(authors);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 