using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class PropertyImage
{
    public Guid Id { get; set; }
    
    [Required]
    public string Url { get; set; } = string.Empty;
    
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public Guid PropertyId { get; set; }
    public Property? Property { get; set; }
}
