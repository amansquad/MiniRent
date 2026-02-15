using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class RentalRecord
{
    public int Id { get; set; }
    
    [Required]
    public int PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    
    [Required]
    [MaxLength(100)]
    public string TenantName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string TenantPhone { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string? TenantEmail { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [Required]
    [Range(0, 10000000)]
    public decimal Deposit { get; set; }
    
    [Required]
    [Range(0, 10000000)]
    public decimal MonthlyRent { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    [Required]
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    
    // Navigation properties
    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
}

public enum RentalStatus
{
    Pending = 0,
    Active = 1,
    Ended = 2,
    Terminated = 3,
    Rejected = 4
}
