namespace MiniRent.Backend.Dtos;

public class DashboardOverviewDto
{
    public int TotalProperties { get; set; }
    public int AvailableProperties { get; set; }
    public int RentedProperties { get; set; }
    public int ActiveRentals { get; set; }
    public int NewInquiries { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public List<RecentActivityDto> RecentActivities { get; set; } = new();
    public List<PropertyStatusCountDto> PropertyStatusCounts { get; set; } = new();
}

public class RecentActivityDto
{
    public string Type { get; set; } = string.Empty; // "property", "rental", "inquiry"
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; } = string.Empty;
}

public class PropertyStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class SearchResultDto
{
    public List<PropertyDto> Properties { get; set; } = new();
    public List<RentalRecordDto> Rentals { get; set; } = new();
    public List<RentalInquiryDto> Inquiries { get; set; } = new();
}

public class GlobalSearchDto
{
    public string Query { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}
