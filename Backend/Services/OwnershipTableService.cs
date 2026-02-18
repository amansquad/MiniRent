using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Data;
using MiniRent.Backend.Models;

namespace MiniRent.Backend.Services;

/// <summary>
/// Service for managing the UserPropertyOwnership table
/// </summary>
public class OwnershipTableService
{
    private readonly AppDbContext _context;

    public OwnershipTableService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Populate the UserPropertyOwnership table with existing property ownership data
    /// </summary>
    public async Task<int> PopulateOwnershipTableAsync()
    {
        var properties = await _context.Properties
            .Where(p => !p.IsDeleted && p.CreatedById != null)
            .ToListAsync();

        int addedCount = 0;

        foreach (var property in properties)
        {
            // Check if ownership record already exists
            var exists = await _context.UserPropertyOwnerships
                .AnyAsync(upo => upo.UserId == property.CreatedById && upo.PropertyId == property.Id);

            if (!exists)
            {
                var ownership = new UserPropertyOwnership
                {
                    UserId = property.CreatedById!.Value,
                    PropertyId = property.Id,
                    OwnershipStartDate = property.CreatedAt,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.UserPropertyOwnerships.Add(ownership);
                addedCount++;
            }
        }

        if (addedCount > 0)
        {
            await _context.SaveChangesAsync();
        }

        return addedCount;
    }

    /// <summary>
    /// Add ownership record for a specific property
    /// </summary>
    public async Task<UserPropertyOwnership?> AddOwnershipRecordAsync(Guid userId, Guid propertyId, string? notes = null)
    {
        // Check if record already exists
        var existing = await _context.UserPropertyOwnerships
            .FirstOrDefaultAsync(upo => upo.UserId == userId && upo.PropertyId == propertyId);

        if (existing != null)
        {
            return existing; // Already exists
        }

        // Verify user and property exist
        var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
        var propertyExists = await _context.Properties.AnyAsync(p => p.Id == propertyId && !p.IsDeleted);

        if (!userExists || !propertyExists)
        {
            return null; // Invalid user or property
        }

        var ownership = new UserPropertyOwnership
        {
            UserId = userId,
            PropertyId = propertyId,
            OwnershipStartDate = DateTime.UtcNow,
            IsActive = true,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.UserPropertyOwnerships.Add(ownership);
        await _context.SaveChangesAsync();

        return ownership;
    }

    /// <summary>
    /// Deactivate ownership record (for ownership transfers)
    /// </summary>
    public async Task<bool> DeactivateOwnershipAsync(Guid userId, Guid propertyId)
    {
        var ownership = await _context.UserPropertyOwnerships
            .FirstOrDefaultAsync(upo => upo.UserId == userId && upo.PropertyId == propertyId && upo.IsActive);

        if (ownership == null)
        {
            return false;
        }

        ownership.IsActive = false;
        ownership.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Get all ownership records for a user
    /// </summary>
    public async Task<List<UserPropertyOwnership>> GetUserOwnershipsAsync(Guid userId)
    {
        return await _context.UserPropertyOwnerships
            .Where(upo => upo.UserId == userId)
            .Include(upo => upo.Property)
            .OrderByDescending(upo => upo.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Get ownership record for a property
    /// </summary>
    public async Task<UserPropertyOwnership?> GetPropertyOwnershipAsync(Guid propertyId)
    {
        return await _context.UserPropertyOwnerships
            .Where(upo => upo.PropertyId == propertyId && upo.IsActive)
            .Include(upo => upo.User)
            .Include(upo => upo.Property)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Check if table exists and has data
    /// </summary>
    public async Task<(bool tableExists, int recordCount)> GetTableStatusAsync()
    {
        try
        {
            var count = await _context.UserPropertyOwnerships.CountAsync();
            return (true, count);
        }
        catch
        {
            return (false, 0);
        }
    }

    /// <summary>
    /// Get count of properties that can be added to ownership table
    /// </summary>
    public async Task<int> GetAvailablePropertiesCountAsync()
    {
        return await _context.Properties
            .Where(p => !p.IsDeleted && p.CreatedById != null)
            .CountAsync();
    }
}
