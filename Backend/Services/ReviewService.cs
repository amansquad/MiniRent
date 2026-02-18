using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class ReviewService : IReviewService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ReviewService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ReviewDto> AddReviewAsync(ReviewCreateDto createDto, Guid reviewerId)
    {
        // Check if property exists
        var propertyExists = await _context.Properties.AnyAsync(p => p.Id == createDto.PropertyId && !p.IsDeleted);
        if (!propertyExists)
        {
            throw new ArgumentException("Property not found.");
        }

        // Check if user has already reviewed this property
        var alreadyReviewed = await _context.Reviews.AnyAsync(r => r.PropertyId == createDto.PropertyId && r.ReviewerId == reviewerId);
        if (alreadyReviewed)
        {
            throw new InvalidOperationException("User has already reviewed this property.");
        }

        // Check if user has completed a rental for this property
        var hasCompletedRental = await _context.RentalRecords.AnyAsync(r => 
            r.PropertyId == createDto.PropertyId && 
            r.TenantId == reviewerId && 
            (r.Status == RentalStatus.Ended || r.Status == RentalStatus.Active));
        
        if (!hasCompletedRental)
        {
            throw new InvalidOperationException("You can only review properties you have rented.");
        }

        var review = _mapper.Map<Review>(createDto);
        review.ReviewerId = reviewerId;
        review.CreatedAt = DateTime.UtcNow;

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Reload with reviewer info
        var createdReview = await _context.Reviews
            .Include(r => r.Reviewer)
            .FirstAsync(r => r.Id == review.Id);

        return _mapper.Map<ReviewDto>(createdReview);
    }

    public async Task<List<ReviewDto>> GetPropertyReviewsAsync(Guid propertyId)
    {
        var reviews = await _context.Reviews
            .Include(r => r.Reviewer)
            .Where(r => r.PropertyId == propertyId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<ReviewDto>>(reviews);
    }

    public async Task<bool> DeleteReviewAsync(Guid reviewId, Guid userId, bool isAdmin = false)
    {
        var review = await _context.Reviews.FindAsync(reviewId);
        if (review == null) return false;

        // Check permission: only admin or the reviewer can delete
        if (!isAdmin && review.ReviewerId != userId)
        {
            return false;
        }

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }
}
