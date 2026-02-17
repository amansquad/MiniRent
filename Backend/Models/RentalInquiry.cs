using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class RentalInquiry
{
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Message { get; set; }
    
    [MaxLength(500)]
    public string? OwnerReply { get; set; }
    
    [Required]
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Optional property link
    public Guid? PropertyId { get; set; }
    public Property? Property { get; set; }
    
    // Navigation properties
    public Guid? CreatedById { get; set; }
    public User? CreatedBy { get; set; }

    public Guid? RentalRecordId { get; set; }
    public RentalRecord? RentalRecord { get; set; }
}

public enum InquiryStatus
{
    New = 0,
    Accepted = 1,
    Rejected = 2,
    Converted = 3
}
