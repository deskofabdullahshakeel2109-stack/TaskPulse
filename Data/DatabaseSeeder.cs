using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskPulse.Api.Models;

namespace TaskPulse.Api.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        var email = (
            configuration["SeedAdmin:Email"]
            ?? "admin@taskpulse.local"
        ).Trim().ToLowerInvariant();

        var password =
            configuration["SeedAdmin:Password"]
            ?? "Admin@12345";

        var fullName =
            configuration["SeedAdmin:FullName"]
            ?? "TaskPulse Administrator";

        var admin = await context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (admin is null)
        {
            admin = new User
            {
                Id = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            admin.PasswordHash =
                passwordHasher.HashPassword(admin, password);

            context.Users.Add(admin);
        }
        else
        {
            // Ensure the configured seed account remains an Admin.
            admin.Role = "Admin";
            admin.FullName = fullName;

            // Reset the password to the configured seed password.
            admin.PasswordHash =
                passwordHasher.HashPassword(admin, password);
        }

        await context.SaveChangesAsync();
    }
}