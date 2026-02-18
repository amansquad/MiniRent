// Run this with: dotnet script CreateAdminUser.cs
// Or add it as a temporary endpoint in your API

using System;
using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Data;
using MiniRent.Backend.Models;

public class AdminUserCreator
{
    public static async Task CreateAdminUser()
    {
        var connectionString = "Server=localhost;Database=MiniRent;User Id=sa;Password=BrrBrrPatapim@123;TrustServerCertificate=True";
        
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        
        using var context = new AppDbContext(optionsBuilder.Options);
        
        // Check if admin exists
        var adminExists = await context.Users.AnyAsync(u => u.Username == "admin");
        
        if (adminExists)
        {
            Console.WriteLine("Admin user already exists!");
            var admin = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            Console.WriteLine($"Admin ID: {admin.Id}");
            Console.WriteLine($"Admin Role: {admin.Role}");
            Console.WriteLine($"Admin IsActive: {admin.IsActive}");
            return;
        }
        
        // Create admin user
        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            FullName = "System Administrator",
            Email = "admin@minirent.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Users.Add(adminUser);
        await context.SaveChangesAsync();
        
        Console.WriteLine("Admin user created successfully!");
        Console.WriteLine($"Username: admin");
        Console.WriteLine($"Password: admin123");
    }
}
