using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

/// <summary>
/// Represents the ownership relationship between a user and a property.
/// This table provides explicit tracking of property ownership.
/// </summary>
public class UserPropertyOwnership
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    [Required]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    
    public DateTime OwnershipStartDate { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    [MaxLength(500)]
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
