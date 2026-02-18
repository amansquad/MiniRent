using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class OwnershipService : IOwnershipService
{
    private readonly AppDbContext _context;

    public OwnershipService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserPropertyOwnershipDto>> GetAllUserPropertyOwnershipsAsync()
    {
        var results = await _context.Database
            .SqlQuery<UserPropertyOwnershipDto>($"SELECT * FROM vw_UserPropertyOwnership")
            .ToListAsync();
        
        return results;
    }

    public async Task<IEnumerable<UserPropertyOwnershipDto>> GetPropertiesByOwnerAsync(Guid userId)
    {
        var results = await _context.Database
            .SqlQueryRaw<UserPropertyOwnershipDto>(@"
                SELECT * FROM vw_UserPropertyOwnership 
                WHERE UserId = {0}
                ORDER BY PropertyCreatedAt DESC", userId)
            .ToListAsync();
        
        return results;
    }

    public async Task<UserPropertyOwnershipDto?> GetPropertyOwnershipAsync(Guid propertyId)
    {
        var result = await _context.Database
            .SqlQueryRaw<UserPropertyOwnershipDto>(@"
                SELECT * FROM vw_UserPropertyOwnership 
                WHERE PropertyId = {0}", propertyId)
            .FirstOrDefaultAsync();
        
        return result;
    }

    public async Task<IEnumerable<UserPropertiesSummaryDto>> GetAllUserPropertiesSummariesAsync()
    {
        var results = await _context.Database
            .SqlQuery<UserPropertiesSummaryDto>($"SELECT * FROM vw_UserPropertiesSummary ORDER BY TotalProperties DESC")
            .ToListAsync();
        
        return results;
    }

    public async Task<UserPropertiesSummaryDto?> GetUserPropertiesSummaryAsync(Guid userId)
    {
        var result = await _context.Database
            .SqlQueryRaw<UserPropertiesSummaryDto>(@"
                SELECT * FROM vw_UserPropertiesSummary 
                WHERE UserId = {0}", userId)
            .FirstOrDefaultAsync();
        
        return result;
    }

    public async Task<IEnumerable<PropertyOwnershipHistoryDto>> GetAllPropertyOwnershipHistoriesAsync()
    {
        var results = await _context.Database
            .SqlQuery<PropertyOwnershipHistoryDto>($"SELECT * FROM vw_PropertyOwnershipHistory ORDER BY OwnershipStartDate DESC")
            .ToListAsync();
        
        return results;
    }

    public async Task<PropertyOwnershipHistoryDto?> GetPropertyOwnershipHistoryAsync(Guid propertyId)
    {
        var result = await _context.Database
            .SqlQueryRaw<PropertyOwnershipHistoryDto>(@"
                SELECT * FROM vw_PropertyOwnershipHistory 
                WHERE PropertyId = {0}", propertyId)
            .FirstOrDefaultAsync();
        
        return result;
    }

    public async Task<IEnumerable<UserPropertyDto>> GetUserPropertiesSimpleAsync(Guid userId)
    {
        var results = await _context.Database
            .SqlQueryRaw<UserPropertyDto>(@"
                SELECT 
                    u.Id AS UserId,
                    u.Username,
                    u.FullName AS OwnerName,
                    p.Id AS PropertyId,
                    p.Address AS PropertyAddress,
                    p.Status AS PropertyStatus,
                    p.MonthlyRent
                FROM Users u
                INNER JOIN Properties p ON u.Id = p.CreatedById
                WHERE u.Id = {0} AND p.IsDeleted = 0
                ORDER BY p.CreatedAt DESC", userId)
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
