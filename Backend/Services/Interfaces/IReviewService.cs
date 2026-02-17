using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> AddReviewAsync(ReviewCreateDto createDto, Guid reviewerId);
    Task<List<ReviewDto>> GetPropertyReviewsAsync(Guid propertyId);
    Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin = false);
}
