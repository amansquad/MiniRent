using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Dtos;

public class PropertyDto
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public int Bedrooms { get; set; }
    public int? Floor { get; set; }
    public decimal MonthlyRent { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? CreatedById { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<RentalRecordDto> RecentRentals { get; set; } = new();
}

public class PropertyCreateDto
{
    [Required]
    [MaxLength(200)]
    public string Address { get; set; } = string.Empty;
    
    [Required]
    [Range(1, 10000)]
    public decimal Area { get; set; }
    
    [Required]
    [Range(1, 20)]
    public int Bedrooms { get; set; }
    
    [Range(0, 50)]
    public int? Floor { get; set; }
    
    [Required]
    [Range(1, 10000000)]
    public decimal MonthlyRent { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
}

public class PropertyUpdateDto : PropertyCreateDto
{
    public int Id { get; set; }
    public string? Status { get; set; }
}

public class PropertyFilterDto
{
    public string? Status { get; set; }
    public int? MinBedrooms { get; set; }
    public int? MaxBedrooms { get; set; }
    public decimal? MinRent { get; set; }
    public decimal? MaxRent { get; set; }
    public string? SearchAddress { get; set; }
    public string? SortBy { get; set; } // "rent" or "date"
    public string? SortOrder { get; set; } // "asc" or "desc"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class PropertyStatusUpdateDto
{
    [Required]
    public string Status { get; set; } = string.Empty;
}
