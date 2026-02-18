using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

/// <summary>
/// Simple ownership service that works without the UserPropertyOwnership table
/// Uses the existing Properties.CreatedById relationship
/// </summary>
public class SimpleOwnershipService : IOwnershipService
{
    private readonly AppDbContext _context;

    public SimpleOwnershipService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserPropertyOwnershipDto>> GetAllUserPropertyOwnershipsAsync()
    {
        var results = await _context.Properties
            .Where(p => !p.IsDeleted && p.CreatedById != null)
            .Include(p => p.CreatedBy)
            .Include(p => p.RentalHistory)
            .Select(p => new UserPropertyOwnershipDto
            {
                UserId = p.CreatedById!.Value,
                Username = p.CreatedBy!.Username,
                OwnerName = p.CreatedBy.FullName,
                OwnerEmail = p.CreatedBy.Email,
                OwnerPhone = p.CreatedBy.Phone,
                UserRole = p.CreatedBy.Role.ToString(),
                PropertyId = p.Id,
                PropertyTitle = p.Title,
                PropertyAddress = p.Address,
                City = p.City,
                State = p.State,
                Country = p.Country,
                PropertyType = p.PropertyType,
                PropertyStatus = p.Status.ToString(),
                MonthlyRent = p.MonthlyRent,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Area = p.Area,
                Floor = p.Floor,
                PropertyCreatedAt = p.CreatedAt,
                TotalRentals = p.RentalHistory.Count,
                ActiveRentals = p.RentalHistory.Count(r => r.Status == Models.RentalStatus.Active),
                LastRentalDate = p.RentalHistory.OrderByDescending(r => r.StartDate).FirstOrDefault() != null 
                    ? p.RentalHistory.OrderByDescending(r => r.StartDate).First().StartDate 
                    : null,
                CurrentTenantName = p.RentalHistory
                    .Where(r => r.Status == Models.RentalStatus.Active)
                    .OrderByDescending(r => r.StartDate)
                    .Select(r => r.Tenant.FullName)
                    .FirstOrDefault(),
                CurrentRentalStartDate = p.RentalHistory
                    .Where(r => r.Status == Models.RentalStatus.Active)
                    .OrderByDescending(r => r.StartDate)
                    .Select(r => r.StartDate)
                    .FirstOrDefault()
            })
            .ToListAsync();

        return results;
    }

    public async Task<IEnumerable<UserPropertyOwnershipDto>> GetPropertiesByOwnerAsync(Guid userId)
    {
        var results = await _context.Properties
            .Where(p => !p.IsDeleted && p.CreatedById == userId)
            .Include(p => p.CreatedBy)
            .Include(p => p.RentalHistory)
                .ThenInclude(r => r.Tenant)
            .Select(p => new UserPropertyOwnershipDto
            {
                UserId = p.CreatedById!.Value,
                Username = p.CreatedBy!.Username,
                OwnerName = p.CreatedBy.FullName,
                OwnerEmail = p.CreatedBy.Email,
                OwnerPhone = p.CreatedBy.Phone,
                UserRole = p.CreatedBy.Role.ToString(),
                PropertyId = p.Id,
                PropertyTitle = p.Title,
                PropertyAddress = p.Address,
                City = p.City,
                State = p.State,
                Country = p.Country,
                PropertyType = p.PropertyType,
                PropertyStatus = p.Status.ToString(),
                MonthlyRent = p.MonthlyRent,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Area = p.Area,
                Floor = p.Floor,
                PropertyCreatedAt = p.CreatedAt,
                TotalRentals = p.RentalHistory.Count,
                ActiveRentals = p.RentalHistory.Count(r => r.Status == Models.RentalStatus.Active),
                LastRentalDate = p.RentalHistory.OrderByDescending(r => r.StartDate).FirstOrDefault() != null 
                    ? p.RentalHistory.OrderByDescending(r => r.StartDate).First().StartDate 
                    : null,
                CurrentTenantName = p.RentalHistory
                    .Where(r => r.Status == Models.RentalStatus.Active)
                    .OrderByDescending(r => r.StartDate)
                    .Select(r => r.Tenant.FullName)
                    .FirstOrDefault(),
                CurrentRentalStartDate = p.RentalHistory
                    .Where(r => r.Status == Models.RentalStatus.Active)
                    .OrderByDescending(r => r.StartDate)
                    .Select(r => r.StartDate)
                    .FirstOrDefault()
            })
            .OrderByDescending(p => p.PropertyCreatedAt)
            .ToListAsync();

        return results;
    }

    public async Task<UserPropertyOwnershipDto?> GetPropertyOwnershipAsync(Guid propertyId)
    {
        var result = await _context.Properties
            .Where(p => p.Id == propertyId && !p.IsDeleted && p.CreatedById != null)
            .Include(p => p.CreatedBy)
            .Include(p => p.RentalHistory)
                .ThenInclude(r => r.Tenant)
            .Select(p => new UserPropertyOwnershipDto
            {
                UserId = p.CreatedById!.Value,
                Username = p.CreatedBy!.Username,
                OwnerName = p.CreatedBy.FullName,
                OwnerEmail = p.CreatedBy.Email,
                OwnerPhone = p.CreatedBy.Phone,
                UserRole = p.CreatedBy.Role.ToString(),
                PropertyId = p.Id,
                PropertyTitle = p.Title,
                PropertyAddress = p.Address,
                City = p.City,
                State = p.State,
                Country = p.Country,
                PropertyType = p.PropertyType,
                PropertyStatus = p.Status.ToString(),
                MonthlyRent = p.MonthlyRent,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Area = p.Area,
                Floor = p.Floor,
                PropertyCreatedAt = p.CreatedAt,
                TotalRentals = p.RentalHistory.Count,
                ActiveRentals = p.RentalHistory.Count(r => r.Status == Models.RentalStatus.Active),
                LastRentalDate = p.RentalHistory.OrderByDescending(r => r.StartDate).FirstOrDefault() != null 
                    ? p.RentalHistory.OrderByDescending(r => r.StartDate).First().StartDate 
                    : null,
                CurrentTenantName = p.RentalHistory
                    .Where(r => r.Status == Models.RentalStatus.Active)
                    .OrderByDescending(r => r.StartDate)
                    .Select(r => r.Tenant.FullName)
                    .FirstOrDefault(),
                CurrentRentalStartDate = p.RentalHistory
                    .Where(r => r.Status == Models.RentalStatus.Active)
                    .OrderByDescending(r => r.StartDate)
                    .Select(r => r.StartDate)
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<IEnumerable<UserPropertiesSummaryDto>> GetAllUserPropertiesSummariesAsync()
    {
        var results = await _context.Users
            .Select(u => new UserPropertiesSummaryDto
            {
                UserId = u.Id,
                Username = u.Username,
                OwnerName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                TotalProperties = u.OwnedProperties.Count(p => !p.IsDeleted),
                AvailableProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Available),
                RentedProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Rented),
                ReservedProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Reserved),
                MaintenanceProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Maintenance),
                TotalMonthlyIncome = u.OwnedProperties
                    .Where(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Rented)
                    .Sum(p => p.MonthlyRent),
                FirstPropertyDate = u.OwnedProperties.Any(p => !p.IsDeleted) 
                    ? u.OwnedProperties.Where(p => !p.IsDeleted).Min(p => p.CreatedAt) 
                    : (DateTime?)null,
                LastPropertyDate = u.OwnedProperties.Any(p => !p.IsDeleted) 
                    ? u.OwnedProperties.Where(p => !p.IsDeleted).Max(p => p.CreatedAt) 
                    : (DateTime?)null,
                Cities = null // Will be populated separately if needed
            })
            .OrderByDescending(u => u.TotalProperties)
            .ToListAsync();

        return results;
    }

    public async Task<UserPropertiesSummaryDto?> GetUserPropertiesSummaryAsync(Guid userId)
    {
        var result = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new UserPropertiesSummaryDto
            {
                UserId = u.Id,
                Username = u.Username,
                OwnerName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                TotalProperties = u.OwnedProperties.Count(p => !p.IsDeleted),
                AvailableProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Available),
                RentedProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Rented),
                ReservedProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Reserved),
                MaintenanceProperties = u.OwnedProperties.Count(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Maintenance),
                TotalMonthlyIncome = u.OwnedProperties
                    .Where(p => !p.IsDeleted && p.Status == Models.PropertyStatus.Rented)
                    .Sum(p => p.MonthlyRent),
                FirstPropertyDate = u.OwnedProperties.Any(p => !p.IsDeleted) 
                    ? u.OwnedProperties.Where(p => !p.IsDeleted).Min(p => p.CreatedAt) 
                    : (DateTime?)null,
                LastPropertyDate = u.OwnedProperties.Any(p => !p.IsDeleted) 
                    ? u.OwnedProperties.Where(p => !p.IsDeleted).Max(p => p.CreatedAt) 
                    : (DateTime?)null,
                Cities = string.Join(", ", u.OwnedProperties
                    .Where(p => !p.IsDeleted)
                    .Select(p => p.City)
                    .Distinct()
                    .OrderBy(c => c))
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<IEnumerable<PropertyOwnershipHistoryDto>> GetAllPropertyOwnershipHistoriesAsync()
    {
        var results = await _context.Properties
            .Where(p => !p.IsDeleted && p.CreatedById != null)
            .Include(p => p.CreatedBy)
            .Include(p => p.Inquiries)
            .Include(p => p.Reviews)
            .Include(p => p.RentalHistory)
            .Select(p => new PropertyOwnershipHistoryDto
            {
                PropertyId = p.Id,
                PropertyTitle = p.Title,
                Address = p.Address,
                City = p.City,
                State = p.State,
                Status = p.Status.ToString(),
                MonthlyRent = p.MonthlyRent,
                CurrentOwnerId = p.CreatedById!.Value,
                CurrentOwnerUsername = p.CreatedBy!.Username,
                CurrentOwnerName = p.CreatedBy.FullName,
                CurrentOwnerEmail = p.CreatedBy.Email,
                CurrentOwnerPhone = p.CreatedBy.Phone,
                OwnershipStartDate = p.CreatedAt,
                LastModifiedDate = p.UpdatedAt,
                DaysOwned = (int)(DateTime.UtcNow - p.CreatedAt).TotalDays,
                TotalRentals = p.RentalHistory.Count,
                TotalInquiries = p.Inquiries.Count,
                AverageRating = p.Reviews.Any() ? (decimal?)p.Reviews.Average(r => r.Rating) : null
            })
            .OrderByDescending(p => p.OwnershipStartDate)
            .ToListAsync();

        return results;
    }

    public async Task<PropertyOwnershipHistoryDto?> GetPropertyOwnershipHistoryAsync(Guid propertyId)
    {
        var result = await _context.Properties
            .Where(p => p.Id == propertyId && !p.IsDeleted && p.CreatedById != null)
            .Include(p => p.CreatedBy)
            .Include(p => p.Inquiries)
            .Include(p => p.Reviews)
            .Include(p => p.RentalHistory)
            .Select(p => new PropertyOwnershipHistoryDto
            {
                PropertyId = p.Id,
                PropertyTitle = p.Title,
                Address = p.Address,
                City = p.City,
                State = p.State,
                Status = p.Status.ToString(),
                MonthlyRent = p.MonthlyRent,
                CurrentOwnerId = p.CreatedById!.Value,
                CurrentOwnerUsername = p.CreatedBy!.Username,
                CurrentOwnerName = p.CreatedBy.FullName,
                CurrentOwnerEmail = p.CreatedBy.Email,
                CurrentOwnerPhone = p.CreatedBy.Phone,
                OwnershipStartDate = p.CreatedAt,
                LastModifiedDate = p.UpdatedAt,
                DaysOwned = (int)(DateTime.UtcNow - p.CreatedAt).TotalDays,
                TotalRentals = p.RentalHistory.Count,
                TotalInquiries = p.Inquiries.Count,
                AverageRating = p.Reviews.Any() ? (decimal?)p.Reviews.Average(r => r.Rating) : null
            })
            .FirstOrDefaultAsync();

        return result;
    }

    public async Task<IEnumerable<UserPropertyDto>> GetUserPropertiesSimpleAsync(Guid userId)
    {
        var results = await _context.Properties
            .Where(p => p.CreatedById == userId && !p.IsDeleted)
            .Include(p => p.CreatedBy)
            .Select(p => new UserPropertyDto
            {
                UserId = p.CreatedById!.Value,
                Username = p.CreatedBy!.Username,
                OwnerName = p.CreatedBy.FullName,
                PropertyId = p.Id,
                PropertyAddress = p.Address,
                PropertyStatus = p.Status.ToString(),
                MonthlyRent = p.MonthlyRent
            })
            .OrderByDescending(p => p.MonthlyRent)
            .ToListAsync();

        return results;
    }

    public async Task<int> GetPropertyCountByOwnerAsync(Guid userId)
    {
        var count = await _context.Properties
            .Where(p => p.CreatedById == userId && !p.IsDeleted)
            .CountAsync();

        return count;
    }
}
