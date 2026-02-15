using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class Property
{
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 10000)]
    public decimal Area { get; set; } // in square meters
    
    [Required]
    [Range(1, 20)]
    public int Bedrooms { get; set; }
    
    [Range(0, 50)]
    public int? Floor { get; set; }
    
    [Required]
    [Range(1, 100000)]
    public decimal MonthlyRent { get; set; }
    
    [Required]
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;
    
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    // Navigation properties
    public int? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public int? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }
    
    // Collections
    public ICollection<RentalRecord> RentalHistory { get; set; } = new List<RentalRecord>();
    public ICollection<RentalInquiry> Inquiries { get; set; } = new List<RentalInquiry>();
}

public enum PropertyStatus
{
    Available = 0,
    Rented = 1,
    Reserved = 2,
    Maintenance = 3
}
