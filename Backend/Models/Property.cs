using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class Property
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ZipCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string PropertyType { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 10000)]
    public decimal Area { get; set; } // in square meters
    
    [Required]
    [Range(1, 20)]
    public int Bedrooms { get; set; }

    [Required]
    [Range(0, 20)]
    public double Bathrooms { get; set; }
    
    [Range(0, 50)]
    public int? Floor { get; set; }
    
    [Required]
    [Range(1, 10000000)]
    public decimal MonthlyRent { get; set; }
    
    [Required]
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;
    
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    // Navigation properties
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }
    public Guid? UpdatedById { get; set; }
    public User? UpdatedBy { get; set; }
    
    // Collections
    public ICollection<RentalRecord> RentalHistory { get; set; } = new List<RentalRecord>();
    public ICollection<RentalInquiry> Inquiries { get; set; } = new List<RentalInquiry>();
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}

public enum PropertyStatus
{
    Available = 0,
    Rented = 1,
    Reserved = 2,
    Maintenance = 3
}
