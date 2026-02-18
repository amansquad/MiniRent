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
    public DbSet<PropertyImage> PropertyImages { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<Review> Reviews { get; set; }
    
    // Statistics tables
    public DbSet<UserOwnershipStats> UserOwnershipStats { get; set; }
    public DbSet<PropertyStatistics> PropertyStatistics { get; set; }
    public DbSet<RentalStatistics> RentalStatistics { get; set; }
    
    // Ownership tracking
    public DbSet<UserPropertyOwnership> UserPropertyOwnerships { get; set; }

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
            entity.Property(e => e.Bathrooms).HasColumnType("decimal(4,1)");

            entity.HasOne(e => e.CreatedBy)
                .WithMany(u => u.OwnedProperties)
                .HasForeignKey(e => e.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.UpdatedBy)
                .WithMany()
                .HasForeignKey(e => e.UpdatedById)
                .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasIndex(e => e.Address);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.MonthlyRent);
            entity.HasIndex(e => e.City);
            entity.HasIndex(e => e.ZipCode);
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.State);
            entity.HasIndex(e => e.Country);
            entity.HasIndex(e => e.PropertyType);
        });

        // RentalRecord configuration
        modelBuilder.Entity<RentalRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).HasConversion<string>();
            entity.Property(e => e.SecurityDeposit).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MonthlyRent).HasColumnType("decimal(10,2)");
            
            entity.HasOne(e => e.Property)
                  .WithMany(p => p.RentalHistory)
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tenant)
                  .WithMany(u => u.TenantRentals)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasOne(e => e.CreatedBy)
                  .WithMany()
                  .HasForeignKey(e => e.CreatedById)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.TenantName);
            entity.HasIndex(e => e.StartDate);
            entity.HasIndex(e => e.EndDate);
            entity.HasIndex(e => e.Status);
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
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.RentalRecord)
                  .WithMany()
                  .HasForeignKey(e => e.RentalRecordId)
                  .OnDelete(DeleteBehavior.NoAction);
                  
            entity.HasIndex(e => e.Email);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.CreatedById);
            entity.HasIndex(e => e.RentalRecordId);
        });

        // Amenity configuration
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Review configuration
        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Property)
                  .WithMany(p => p.Reviews)
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Reviewer)
                  .WithMany()
                  .HasForeignKey(e => e.ReviewerId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        // Many-to-Many: Property <-> Amenity
        modelBuilder.Entity<Property>()
            .HasMany(p => p.Amenities)
            .WithMany(a => a.Properties)
            .UsingEntity(j => j.ToTable("PropertyAmenities"));

        // PropertyImage configuration
        modelBuilder.Entity<PropertyImage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.Property)
                  .WithMany(p => p.Images)
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.PropertyId);
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).HasConversion<string>();
            
            entity.HasOne(e => e.Rental)
                  .WithMany(r => r.Payments)
                  .HasForeignKey(e => e.RentalId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.RentalId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.DueDate);
        });

        // Activity configuration
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Activities)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Type);
        });

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // UserOwnershipStats configuration
        modelBuilder.Entity<UserOwnershipStats>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalMonthlyIncome).HasColumnType("decimal(18,2)");
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.UserId).IsUnique();
        });

        // PropertyStatistics configuration
        modelBuilder.Entity<PropertyStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalRevenue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OccupancyRate).HasColumnType("decimal(5,2)");
            entity.Property(e => e.AverageRating).HasColumnType("decimal(3,2)");
            
            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.PropertyId).IsUnique();
        });

        // RentalStatistics configuration
        modelBuilder.Entity<RentalStatistics>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalPaid).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPending).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalOverdue).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentCompletionRate).HasColumnType("decimal(5,2)");
            
            entity.HasOne(e => e.Rental)
                  .WithMany()
                  .HasForeignKey(e => e.RentalId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.RentalId).IsUnique();
        });

        // UserPropertyOwnership configuration
        modelBuilder.Entity<UserPropertyOwnership>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne(e => e.Property)
                  .WithMany()
                  .HasForeignKey(e => e.PropertyId)
                  .OnDelete(DeleteBehavior.NoAction);
            
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.PropertyId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.UserId, e.PropertyId }).IsUnique();
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Create default admin user
        var adminUser = new User
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
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
