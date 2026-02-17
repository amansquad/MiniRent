namespace MiniRent.Backend.Dtos;

public class AmenityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}

public class AmenityCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
}
