using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Dtos;

/// <summary>
/// Data transfer object for property details.
/// </summary>
public class PropertyDto
{
    public Guid Id { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Area { get; set; }
    public int Bedrooms { get; set; }
    public int? Floor { get; set; }
    public decimal MonthlyRent { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public double Bathrooms { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid? CreatedById { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public List<RentalRecordDto> RecentRentals { get; set; } = new();
    public List<PropertyImageDto> Images { get; set; } = new();
    public List<AmenityDto> Amenities { get; set; } = new();
    public List<ReviewDto> Reviews { get; set; } = new();
}

/// <summary>
/// Data transfer object for property creation.
/// </summary>
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

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

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
    [Range(0, 20)]
    public double Bathrooms { get; set; }
}

public class PropertyUpdateDto : PropertyCreateDto
{
    public Guid Id { get; set; }
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
