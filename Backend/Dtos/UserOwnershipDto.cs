namespace MiniRent.Backend.Dtos;

public class UserOwnershipDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    
    // Property ownership
    public int TotalPropertiesOwned { get; set; }
    public int AvailableProperties { get; set; }
    public int RentedProperties { get; set; }
    public int ReservedProperties { get; set; }
    public int MaintenanceProperties { get; set; }
    public decimal TotalMonthlyIncome { get; set; }
    
    // Rental as tenant
    public int TotalRentalsAsTenant { get; set; }
    public int ActiveRentalsAsTenant { get; set; }
    public int EndedRentalsAsTenant { get; set; }
    public int PendingRentalsAsTenant { get; set; }
    public decimal TotalMonthlyExpense { get; set; }
}

public class PropertyStatisticsDto
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    public int TotalInquiries { get; set; }
    public int PendingInquiries { get; set; }
    public int TotalReviews { get; set; }
    public decimal? AverageRating { get; set; }
    public int TotalImages { get; set; }
    public int TotalAmenities { get; set; }
    
    public DateTime? LastRentalDate { get; set; }
}

public class RentalStatisticsDto
{
    public Guid RentalId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string RentalStatus { get; set; } = string.Empty;
    
    public int TotalPayments { get; set; }
    public int PaidPayments { get; set; }
    public int PendingPayments { get; set; }
    public int OverduePayments { get; set; }
    
    public decimal TotalPaid { get; set; }
    public decimal TotalPending { get; set; }
    public decimal TotalOverdue { get; set; }
    public decimal PaymentCompletionRate { get; set; }
    
    public DateTime? LastPaymentDate { get; set; }
    public DateTime? NextPaymentDue { get; set; }
}
