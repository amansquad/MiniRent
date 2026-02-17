namespace MiniRent.Backend.Dtos;

public class ReviewDto
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public Guid PropertyId { get; set; }
    public Guid ReviewerId { get; set; }
    public string ReviewerName { get; set; } = string.Empty;
}

public class ReviewCreateDto
{
    public Guid PropertyId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
