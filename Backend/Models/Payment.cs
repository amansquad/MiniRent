using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class Payment
{
    public Guid Id { get; set; }
    
    [Range(0.01, 1000000.00)]
    public decimal Amount { get; set; }
    
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    
    [Required]
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public Guid RentalId { get; set; }
    public RentalRecord? Rental { get; set; }
}

public enum PaymentStatus
{
    Pending = 0,
    Paid = 1,
    Overdue = 2,
    Cancelled = 3
}
