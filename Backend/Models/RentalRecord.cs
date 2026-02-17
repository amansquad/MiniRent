using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class RentalRecord
{
    public Guid Id { get; set; }
    
    [Required]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    
    [Required]
    public Guid TenantId { get; set; }
    public User Tenant { get; set; } = null!;

    [MaxLength(100)]
    public string? TenantName { get; set; }
    
    [MaxLength(20)]
    public string? TenantPhone { get; set; }
    
    [MaxLength(100)]
    public string? TenantEmail { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    [Required]
    [Range(0, 10000000)]
    public decimal SecurityDeposit { get; set; }
    
    [Required]
    [Range(0, 10000000)]
    public decimal MonthlyRent { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    [Required]
    public RentalStatus Status { get; set; } = RentalStatus.Active;
    
    // Navigation properties
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    // Collections
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public enum RentalStatus
{
    Pending = 0,
    Active = 1,
    Ended = 2,
    Terminated = 3,
    Rejected = 4
}
