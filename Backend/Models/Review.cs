using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class Review
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    [Required]
    public Guid PropertyId { get; set; }
    public Property Property { get; set; } = null!;

    [Required]
    public Guid ReviewerId { get; set; }
    public User Reviewer { get; set; } = null!;
}
