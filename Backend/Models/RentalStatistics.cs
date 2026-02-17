using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class RentalStatistics
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public Guid RentalId { get; set; }
    public RentalRecord Rental { get; set; } = null!;
    
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
    
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
