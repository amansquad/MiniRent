namespace MiniRent.Backend.Dtos;

/// <summary>
/// Detailed view of user-property ownership with rental information
/// </summary>
public class UserPropertyOwnershipDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string OwnerEmail { get; set; } = string.Empty;
    public string OwnerPhone { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;
    
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string PropertyAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string PropertyStatus { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    public int Bedrooms { get; set; }
    public double Bathrooms { get; set; }
    public decimal Area { get; set; }
    public int? Floor { get; set; }
    public DateTime PropertyCreatedAt { get; set; }
    
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public DateTime? LastRentalDate { get; set; }
    public string? CurrentTenantName { get; set; }
    public DateTime? CurrentRentalStartDate { get; set; }
}

/// <summary>
/// Summary of properties owned by a user
/// </summary>
public class UserPropertiesSummaryDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    public int TotalProperties { get; set; }
    public int AvailableProperties { get; set; }
    public int RentedProperties { get; set; }
    public int ReservedProperties { get; set; }
    public int MaintenanceProperties { get; set; }
    public decimal TotalMonthlyIncome { get; set; }
    
    public DateTime? FirstPropertyDate { get; set; }
    public DateTime? LastPropertyDate { get; set; }
    public string? Cities { get; set; }
}

/// <summary>
/// Property ownership history with owner details
/// </summary>
public class PropertyOwnershipHistoryDto
{
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
    
    public Guid CurrentOwnerId { get; set; }
    public string CurrentOwnerUsername { get; set; } = string.Empty;
    public string CurrentOwnerName { get; set; } = string.Empty;
    public string CurrentOwnerEmail { get; set; } = string.Empty;
    public string CurrentOwnerPhone { get; set; } = string.Empty;
    
    public DateTime OwnershipStartDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public int DaysOwned { get; set; }
    
    public int TotalRentals { get; set; }
    public int TotalInquiries { get; set; }
    public decimal? AverageRating { get; set; }
}

/// <summary>
/// Simple user-property relationship
/// </summary>
public class UserPropertyDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    
    public Guid PropertyId { get; set; }
    public string PropertyAddress { get; set; } = string.Empty;
    public string PropertyStatus { get; set; } = string.Empty;
    public decimal MonthlyRent { get; set; }
}
