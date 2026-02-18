using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IOwnershipService
{
    // User-Property Ownership queries
    Task<IEnumerable<UserPropertyOwnershipDto>> GetAllUserPropertyOwnershipsAsync();
    Task<IEnumerable<UserPropertyOwnershipDto>> GetPropertiesByOwnerAsync(Guid userId);
    Task<UserPropertyOwnershipDto?> GetPropertyOwnershipAsync(Guid propertyId);
    
    // User Properties Summary
    Task<IEnumerable<UserPropertiesSummaryDto>> GetAllUserPropertiesSummariesAsync();
    Task<UserPropertiesSummaryDto?> GetUserPropertiesSummaryAsync(Guid userId);
    
    // Property Ownership History
    Task<IEnumerable<PropertyOwnershipHistoryDto>> GetAllPropertyOwnershipHistoriesAsync();
    Task<PropertyOwnershipHistoryDto?> GetPropertyOwnershipHistoryAsync(Guid propertyId);
    
    // Simple queries
    Task<IEnumerable<UserPropertyDto>> GetUserPropertiesSimpleAsync(Guid userId);
    Task<int> GetPropertyCountByOwnerAsync(Guid userId);
}
