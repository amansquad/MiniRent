using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Models;

namespace MiniRent.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Property> Properties { get; set; }
    public DbSet<RentalRecord> RentalRecords { get; set; }
    public DbSet<RentalInquiry> RentalInquiries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

          // Property configuration
          modelBuilder.Entity<Property>(entity =>
          {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Area).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MonthlyRent).HasColumnType("decimal(10,2)");

            entity.HasOne(e => e.CreatedBy)
                .WithMany()
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
            
            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasOne(e => e.UpdatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasIndex(e => e.Address);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.MonthlyRent);
        });

        // RentalRecord configuration
        modelBuilder.Entity<RentalRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.Deposit).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MonthlyRent).HasColumnType("decimal(10,2)");
            
            entity.HasOne(e => e.Property)
                  .WithMany(p => p.RentalHistory)
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.TenantName);
            entity.HasIndex(e => e.StartDate);
        });

        // RentalInquiry configuration
        modelBuilder.Entity<RentalInquiry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasOne(e => e.Property)
                  .WithMany(p => p.Inquiries)
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.SetNull);
                  
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Create default admin user
        var adminUser = new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            FullName = "System Administrator",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        modelBuilder.Entity<User>().HasData(adminUser);
    }
}
