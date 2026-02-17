using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class Activity
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
