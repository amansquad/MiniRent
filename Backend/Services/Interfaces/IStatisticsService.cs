using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IStatisticsService
{
    Task<UserOwnershipDto?> GetUserOwnershipStatsAsync(Guid userId);
    Task<IEnumerable<UserOwnershipDto>> GetAllUserOwnershipStatsAsync();
    Task<PropertyStatisticsDto?> GetPropertyStatisticsAsync(Guid propertyId);
    Task<IEnumerable<PropertyStatisticsDto>> GetAllPropertyStatisticsAsync();
    Task<RentalStatisticsDto?> GetRentalStatisticsAsync(Guid rentalId);
    Task<IEnumerable<RentalStatisticsDto>> GetAllRentalStatisticsAsync();
    Task RefreshUserOwnershipStatsAsync(Guid userId);
    Task RefreshPropertyStatisticsAsync(Guid propertyId);
    Task RefreshRentalStatisticsAsync(Guid rentalId);
    Task RefreshAllStatisticsAsync();
}
