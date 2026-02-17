using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class StatisticsService : IStatisticsService
{
    private readonly AppDbContext _context;

    public StatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserOwnershipDto?> GetUserOwnershipStatsAsync(Guid userId)
    {
        var result = await _context.Database
            .SqlQueryRaw<UserOwnershipDto>(@"
                SELECT * FROM vw_UserOwnership WHERE UserId = {0}", userId)
            .FirstOrDefaultAsync();
        
        return result;
    }

    public async Task<IEnumerable<UserOwnershipDto>> GetAllUserOwnershipStatsAsync()
    {
        var results = await _context.Database
            .SqlQuery<UserOwnershipDto>($"SELECT * FROM vw_UserOwnership")
            .ToListAsync();
        
        return results;
    }

    public async Task<PropertyStatisticsDto?> GetPropertyStatisticsAsync(Guid propertyId)
    {
        var result = await _context.Database
            .SqlQueryRaw<PropertyStatisticsDto>(@"
                SELECT 
                    PropertyId, Title, Address, Status,
                    TotalRentals, ActiveRentals, TotalInquiries, PendingInquiries,
                    TotalReviews, AverageRating, TotalImages, TotalAmenities, LastRentalDate
                FROM vw_PropertyDetails 
                WHERE PropertyId = {0}", propertyId)
            .FirstOrDefaultAsync();
        
        return result;
    }

    public async Task<IEnumerable<PropertyStatisticsDto>> GetAllPropertyStatisticsAsync()
    {
        var results = await _context.Database
            .SqlQuery<PropertyStatisticsDto>($@"
                SELECT 
                    PropertyId, Title, Address, Status,
                    TotalRentals, ActiveRentals, TotalInquiries, PendingInquiries,
                    TotalReviews, AverageRating, TotalImages, TotalAmenities, LastRentalDate
                FROM vw_PropertyDetails")
            .ToListAsync();
        
        return results;
    }

    public async Task<RentalStatisticsDto?> GetRentalStatisticsAsync(Guid rentalId)
    {
        var result = await _context.Database
            .SqlQueryRaw<RentalStatisticsDto>(@"
                SELECT 
                    RentalId, PropertyTitle, TenantName, RentalStatus,
                    TotalPayments, PaidPayments, PendingPayments, OverduePayments,
                    TotalPaid, TotalPending, TotalOverdue, 
                    CASE WHEN TotalPayments > 0 THEN (CAST(PaidPayments AS DECIMAL) / TotalPayments * 100) ELSE 0 END AS PaymentCompletionRate,
                    LastPaymentDate, NextPaymentDue
                FROM vw_RentalDetails 
                WHERE RentalId = {0}", rentalId)
            .FirstOrDefaultAsync();
        
        return result;
    }

    public async Task<IEnumerable<RentalStatisticsDto>> GetAllRentalStatisticsAsync()
    {
        var results = await _context.Database
            .SqlQuery<RentalStatisticsDto>($@"
                SELECT 
                    RentalId, PropertyTitle, TenantName, RentalStatus,
                    TotalPayments, PaidPayments, PendingPayments, OverduePayments,
                    TotalPaid, TotalPending, TotalOverdue,
                    CASE WHEN TotalPayments > 0 THEN (CAST(PaidPayments AS DECIMAL) / TotalPayments * 100) ELSE 0 END AS PaymentCompletionRate,
                    LastPaymentDate, NextPaymentDue
                FROM vw_RentalDetails")
            .ToListAsync();
        
        return results;
    }

    public async Task RefreshUserOwnershipStatsAsync(Guid userId)
    {
        var stats = await _context.UserOwnershipStats.FirstOrDefaultAsync(s => s.UserId == userId);
        
        var properties = await _context.Properties
            .Where(p => p.CreatedById == userId && !p.IsDeleted)
            .ToListAsync();
        
        var rentals = await _context.RentalRecords
            .Where(r => r.TenantId == userId)
            .ToListAsync();

        if (stats == null)
        {
            stats = new UserOwnershipStats { UserId = userId };
            _context.UserOwnershipStats.Add(stats);
        }

        stats.TotalProperties = properties.Count;
        stats.AvailableProperties = properties.Count(p => p.Status == PropertyStatus.Available);
        stats.RentedProperties = properties.Count(p => p.Status == PropertyStatus.Rented);
        stats.ReservedProperties = properties.Count(p => p.Status == PropertyStatus.Reserved);
        stats.MaintenanceProperties = properties.Count(p => p.Status == PropertyStatus.Maintenance);
        stats.TotalMonthlyIncome = properties.Where(p => p.Status == PropertyStatus.Rented).Sum(p => p.MonthlyRent);
        
        stats.TotalRentalsAsTenant = rentals.Count;
        stats.ActiveRentalsAsTenant = rentals.Count(r => r.Status == RentalStatus.Active);
        stats.EndedRentalsAsTenant = rentals.Count(r => r.Status == RentalStatus.Ended);
        stats.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RefreshPropertyStatisticsAsync(Guid propertyId)
    {
        var stats = await _context.PropertyStatistics.FirstOrDefaultAsync(s => s.PropertyId == propertyId);
        
        var rentals = await _context.RentalRecords.Where(r => r.PropertyId == propertyId).ToListAsync();
        var inquiries = await _context.RentalInquiries.Where(i => i.PropertyId == propertyId).ToListAsync();
        var reviews = await _context.Reviews.Where(r => r.PropertyId == propertyId).ToListAsync();

        if (stats == null)
        {
            stats = new PropertyStatistics { PropertyId = propertyId };
            _context.PropertyStatistics.Add(stats);
        }

        stats.TotalRentals = rentals.Count;
        stats.ActiveRentals = rentals.Count(r => r.Status == RentalStatus.Active);
        stats.TotalInquiries = inquiries.Count;
        stats.PendingInquiries = inquiries.Count(i => i.Status == InquiryStatus.New);
        stats.TotalReviews = reviews.Count;
        stats.AverageRating = reviews.Any() ? (decimal)reviews.Average(r => r.Rating) : null;
        stats.LastRentalDate = rentals.OrderByDescending(r => r.StartDate).FirstOrDefault()?.StartDate;
        stats.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RefreshRentalStatisticsAsync(Guid rentalId)
    {
        var stats = await _context.RentalStatistics.FirstOrDefaultAsync(s => s.RentalId == rentalId);
        
        var payments = await _context.Payments.Where(p => p.RentalId == rentalId).ToListAsync();

        if (stats == null)
        {
            stats = new RentalStatistics { RentalId = rentalId };
            _context.RentalStatistics.Add(stats);
        }

        stats.TotalPayments = payments.Count;
        stats.PaidPayments = payments.Count(p => p.Status == PaymentStatus.Paid);
        stats.PendingPayments = payments.Count(p => p.Status == PaymentStatus.Pending);
        stats.OverduePayments = payments.Count(p => p.Status == PaymentStatus.Overdue);
        stats.TotalPaid = payments.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount);
        stats.TotalPending = payments.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount);
        stats.TotalOverdue = payments.Where(p => p.Status == PaymentStatus.Overdue).Sum(p => p.Amount);
        stats.PaymentCompletionRate = stats.TotalPayments > 0 ? (decimal)stats.PaidPayments / stats.TotalPayments * 100 : 0;
        stats.LastPaymentDate = payments.Where(p => p.Status == PaymentStatus.Paid).OrderByDescending(p => p.PaidDate).FirstOrDefault()?.PaidDate;
        stats.NextPaymentDue = payments.Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Overdue).OrderBy(p => p.DueDate).FirstOrDefault()?.DueDate;
        stats.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task RefreshAllStatisticsAsync()
    {
        var userIds = await _context.Users.Select(u => u.Id).ToListAsync();
        foreach (var userId in userIds)
        {
            await RefreshUserOwnershipStatsAsync(userId);
        }

        var propertyIds = await _context.Properties.Where(p => !p.IsDeleted).Select(p => p.Id).ToListAsync();
        foreach (var propertyId in propertyIds)
        {
            await RefreshPropertyStatisticsAsync(propertyId);
        }

        var rentalIds = await _context.RentalRecords.Select(r => r.Id).ToListAsync();
        foreach (var rentalId in rentalIds)
        {
            await RefreshRentalStatisticsAsync(rentalId);
        }
    }
}
