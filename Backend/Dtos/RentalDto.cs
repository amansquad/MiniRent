using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Dtos;

public class RentalRecordDto
{
    public Guid Id { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyAddress { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;

    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string TenantPhone { get; set; } = string.Empty;
    public string? TenantEmail { get; set; }
    public Guid TenantId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal SecurityDeposit { get; set; }
    public decimal MonthlyRent { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public Guid? OwnerId { get; set; }
    public List<PaymentDto> Payments { get; set; } = new();
}

public class RentalCreateDto
{
    [Required]
    public Guid PropertyId { get; set; }

    public Guid? InquiryId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string TenantName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string TenantPhone { get; set; } = string.Empty;
    
    [EmailAddress]
    [MaxLength(100)]
    public string? TenantEmail { get; set; }
    
    [Required]
    public Guid TenantId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }
    
    [Required]
    [Range(0, 10000000)]
    public decimal SecurityDeposit { get; set; }
    
    [Required]
    [Range(0, 10000000)]
    public decimal MonthlyRent { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class RentalUpdateDto
{
    [Required]
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string? TenantName { get; set; }
    
    [StringLength(10)]
    [RegularExpression(@"^\d{0,10}$", ErrorMessage = "Phone number must be up to 10 digits.")]
    public string? TenantPhone { get; set; }
    
    [EmailAddress]
    [MaxLength(100)]
    public string? TenantEmail { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
    
    [Range(0, 10000000)]
    public decimal? SecurityDeposit { get; set; }
    
    [Range(0, 10000000)]
    public decimal? MonthlyRent { get; set; }
    
    public string? Status { get; set; }
}

public class RentalEndDto
{
    [Required]
    public DateTime EndDate { get; set; }
    
    [MaxLength(1000)]
    public string? Notes { get; set; }
}

public class RentalFilterDto
{
    public Guid? PropertyId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public string? TenantName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
