using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;
using System.Security.Claims;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    [HttpGet("property/{propertyId}")]
    public async Task<ActionResult<List<ReviewDto>>> GetPropertyReviews(Guid propertyId)
    {
        return Ok(await _reviewService.GetPropertyReviewsAsync(propertyId));
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ReviewDto>> AddReview(ReviewCreateDto createDto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            var review = await _reviewService.AddReviewAsync(createDto, userId);
            return Ok(review);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var isAdmin = User.IsInRole("Admin");

        var success = await _reviewService.DeleteReviewAsync(id, userId, isAdmin);
        return success ? NoContent() : NotFound();
    }
}
