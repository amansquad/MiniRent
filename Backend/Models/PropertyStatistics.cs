using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class PropertyStatistics
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;
    
    public int TotalRentals { get; set; }
    public int ActiveRentals { get; set; }
    
    public int TotalInquiries { get; set; }
    public int PendingInquiries { get; set; }
    
    public int TotalReviews { get; set; }
    public decimal? AverageRating { get; set; }
    
    public decimal TotalRevenue { get; set; }
    public decimal OccupancyRate { get; set; }
    
    public DateTime? LastRentalDate { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
