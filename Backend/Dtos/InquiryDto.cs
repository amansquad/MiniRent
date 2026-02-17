using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Dtos;

public class RentalInquiryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Message { get; set; }
    public string? OwnerReply { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? PropertyId { get; set; }
    public string? PropertyAddress { get; set; }
    public Guid? CreatedById { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public Guid? OwnerId { get; set; }
    public Guid? RentalRecordId { get; set; }
}

public class InquiryCreateDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string Phone { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string? Message { get; set; }
    
    public Guid? PropertyId { get; set; }
}

public class InquiryUpdateDto
{
    public Guid Id { get; set; }
    
    [Required]
    public string Status { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? OwnerReply { get; set; }
}

public class InquiryFilterDto
{
    public string? Status { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? PropertyId { get; set; }
    public string? SearchName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
