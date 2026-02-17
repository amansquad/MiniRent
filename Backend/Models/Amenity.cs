using System.ComponentModel.DataAnnotations;

namespace MiniRent.Backend.Models;

public class Amenity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Icon { get; set; }

    // Relationships
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
