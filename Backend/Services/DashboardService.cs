using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public DashboardService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<DashboardOverviewDto> GetDashboardOverviewAsync(int? userId = null, bool isAdmin = false)
    {
        var now = DateTime.UtcNow;
        // Ensure all DateTimes used in queries are UTC kinds,
        // because PostgreSQL 'timestamp with time zone' only supports UTC with this Npgsql setup.
        var firstDayOfMonth = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1); // end of month, still UTC

        // Base queries
        var propertiesQuery = _context.Properties.Where(p => !p.IsDeleted);
        var rentalsQuery = _context.RentalRecords.Include(r => r.Property).AsQueryable();
        var inquiriesQuery = _context.RentalInquiries.Include(i => i.Property).AsQueryable();

        // If not admin, filter by user's own data (created by user OR user's property)
        if (!isAdmin && userId.HasValue)
        {
            propertiesQuery = propertiesQuery.Where(p => p.CreatedById == userId.Value);
            
            // For rentals and inquiries, include if user created them OR if user owns the property
            rentalsQuery = rentalsQuery.Where(r => r.CreatedById == userId.Value || (r.Property != null && r.Property.CreatedById == userId.Value));
            inquiriesQuery = inquiriesQuery.Where(i => i.CreatedById == userId.Value || (i.Property != null && i.Property.CreatedById == userId.Value));
        }

        // Get property counts
        var totalProperties = await propertiesQuery.CountAsync();

        var availableProperties = await propertiesQuery
            .CountAsync(p => p.Status == PropertyStatus.Available);

        var rentedProperties = await propertiesQuery
            .CountAsync(p => p.Status == PropertyStatus.Rented);

        // Get active rentals (explicitly only Active status)
        var activeRentals = await rentalsQuery
            .CountAsync(r => r.Status == RentalStatus.Active);

        // Get new inquiries (last 7 days)
        var sevenDaysAgo = now.AddDays(-7);
        var newInquiries = await inquiriesQuery
            .CountAsync(i => i.CreatedAt >= sevenDaysAgo);

        // Calculate monthly revenue
        decimal monthlyRevenue = 0m;
        try
        {
            var revenueQuery = rentalsQuery
                .Where(r => r.Status == RentalStatus.Active &&
                           r.StartDate <= lastDayOfMonth &&
                           (!r.EndDate.HasValue || r.EndDate.Value >= firstDayOfMonth));
            monthlyRevenue = await revenueQuery.SumAsync(r => r.MonthlyRent);
        }
        catch (Exception ex)
        {
            // Fallback: if there is any DateTime kind / Npgsql issue, don't break the dashboard.
            Console.WriteLine($"Failed to calculate monthly revenue: {ex}");
            monthlyRevenue = 0m;
        }

        // Get property status counts
        var propertyStatusCounts = await propertiesQuery
            .GroupBy(p => p.Status)
            .Select(g => new PropertyStatusCountDto
            {
                Status = g.Key.ToString(),
                Count = g.Count()
            })
            .ToListAsync();

        // Get recent activities
        var recentActivities = new List<RecentActivityDto>();

        // Recent properties
        var recentPropertiesQuery = _context.Properties
            .Include(p => p.CreatedBy)
            .Where(p => !p.IsDeleted && p.CreatedAt >= sevenDaysAgo);
        
        if (!isAdmin && userId.HasValue)
        {
            recentPropertiesQuery = recentPropertiesQuery.Where(p => p.CreatedById == userId.Value);
        }

        var recentProperties = await recentPropertiesQuery
            .OrderByDescending(p => p.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var property in recentProperties)
        {
            recentActivities.Add(new RecentActivityDto
            {
                Type = "property",
                Description = $"Added new property: {property.Address}",
                Timestamp = property.CreatedAt,
                UserName = property.CreatedBy?.FullName ?? "Unknown"
            });
        }

        // Recent rentals
        var recentRentalsQuery = _context.RentalRecords
            .Include(r => r.CreatedBy)
            .Where(r => r.CreatedAt >= sevenDaysAgo);
        
        if (!isAdmin && userId.HasValue)
        {
            recentRentalsQuery = recentRentalsQuery.Where(r => r.CreatedById == userId.Value || r.Property.CreatedById == userId.Value);
        }

        var recentRentals = await recentRentalsQuery
            .OrderByDescending(r => r.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var rental in recentRentals)
        {
            recentActivities.Add(new RecentActivityDto
            {
                Type = "rental",
                Description = $"Created rental for {rental.TenantName}",
                Timestamp = rental.CreatedAt,
                UserName = rental.CreatedBy?.FullName ?? "Unknown"
            });
        }

        // Recent inquiries
        var recentInquiriesQuery = _context.RentalInquiries
            .Include(i => i.CreatedBy)
            .Where(i => i.CreatedAt >= sevenDaysAgo);
        
        if (!isAdmin && userId.HasValue)
        {
            recentInquiriesQuery = recentInquiriesQuery.Where(i => i.CreatedById == userId.Value || (i.Property != null && i.Property.CreatedById == userId.Value));
        }

        var recentInquiries = await recentInquiriesQuery
            .OrderByDescending(i => i.CreatedAt)
            .Take(3)
            .ToListAsync();

        foreach (var inquiry in recentInquiries)
        {
            recentActivities.Add(new RecentActivityDto
            {
                Type = "inquiry",
                Description = $"New inquiry from {inquiry.Name}",
                Timestamp = inquiry.CreatedAt,
                UserName = inquiry.CreatedBy?.FullName ?? "Public"
            });
        }

        // Sort all activities by timestamp
        recentActivities = recentActivities
            .OrderByDescending(a => a.Timestamp)
            .Take(10)
            .ToList();

        return new DashboardOverviewDto
        {
            TotalProperties = totalProperties,
            AvailableProperties = availableProperties,
            RentedProperties = rentedProperties,
            ActiveRentals = activeRentals,
            NewInquiries = newInquiries,
            MonthlyRevenue = monthlyRevenue,
            RecentActivities = recentActivities,
            PropertyStatusCounts = propertyStatusCounts
        };
    }

    public async Task<SearchResultDto> GlobalSearchAsync(GlobalSearchDto searchDto, int? userId = null, bool isAdmin = false)
    {
        var query = searchDto.Query.ToLower();
        var limit = Math.Min(searchDto.Limit, 20); // Cap at 20 results per type

        var result = new SearchResultDto();

        // Search properties
        var propertiesQuery = _context.Properties
            .Include(p => p.CreatedBy)
            .Where(p => !p.IsDeleted &&
                       (p.Address.ToLower().Contains(query) ||
                        p.Description!.ToLower().Contains(query)));

        if (!isAdmin && userId.HasValue)
        {
            propertiesQuery = propertiesQuery.Where(p => p.CreatedById == userId.Value);
        }

        var properties = await propertiesQuery
            .OrderByDescending(p => p.CreatedAt)
            .Take(limit)
            .ToListAsync();

        result.Properties = _mapper.Map<List<PropertyDto>>(properties);

        // Search rentals
        var rentalsQuery = _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Where(r => r.TenantName.ToLower().Contains(query) ||
                        r.Property.Address.ToLower().Contains(query));

        if (!isAdmin && userId.HasValue)
        {
            rentalsQuery = rentalsQuery.Where(r => r.CreatedById == userId.Value);
        }

        var rentals = await rentalsQuery
            .OrderByDescending(r => r.CreatedAt)
            .Take(limit)
            .ToListAsync();

        result.Rentals = _mapper.Map<List<RentalRecordDto>>(rentals);

        // Search inquiries
        var inquiriesQuery = _context.RentalInquiries
            .Include(i => i.Property)
            .Include(i => i.CreatedBy)
            .Where(i => i.Name.ToLower().Contains(query) ||
                        i.Email.ToLower().Contains(query) ||
                        i.Phone.Contains(query));

        if (!isAdmin && userId.HasValue)
        {
            inquiriesQuery = inquiriesQuery.Where(i => i.CreatedById == userId.Value);
        }

        var inquiries = await inquiriesQuery
            .OrderByDescending(i => i.CreatedAt)
            .Take(limit)
            .ToListAsync();

        result.Inquiries = _mapper.Map<List<RentalInquiryDto>>(inquiries);

        return result;
    }
}
