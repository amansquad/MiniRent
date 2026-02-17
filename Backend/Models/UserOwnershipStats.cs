using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class UserOwnershipStats
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int TotalProperties { get; set; }
    public int AvailableProperties { get; set; }
    public int RentedProperties { get; set; }
    public int ReservedProperties { get; set; }
    public int MaintenanceProperties { get; set; }
    
    public int TotalRentalsAsTenant { get; set; }
    public int ActiveRentalsAsTenant { get; set; }
    public int EndedRentalsAsTenant { get; set; }
    
    public decimal TotalMonthlyIncome { get; set; }
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
