using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Collections
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<Property> OwnedProperties { get; set; } = new List<Property>();
    public ICollection<RentalRecord> TenantRentals { get; set; } = new List<RentalRecord>();
}

public enum UserRole
{
    Admin = 0,
    Agent = 1,
    Tenant = 2
}
