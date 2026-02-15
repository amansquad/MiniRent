namespace MiniRent.Backend.Dtos;

public class UnifiedSearchResultDto
{
    public string Type { get; set; } = string.Empty; // Property, Inquiry, Rental
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
