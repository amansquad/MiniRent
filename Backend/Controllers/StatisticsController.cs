using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsController(IStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    /// <summary>
    /// Get ownership statistics for a specific user
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetUserOwnershipStats(Guid userId)
    {
        var stats = await _statisticsService.GetUserOwnershipStatsAsync(userId);
        if (stats == null)
            return NotFound(new { message = "User statistics not found" });
        
        return Ok(stats);
    }

    /// <summary>
    /// Get ownership statistics for all users
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUserOwnershipStats()
    {
        var stats = await _statisticsService.GetAllUserOwnershipStatsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Get statistics for a specific property
    /// </summary>
    [HttpGet("properties/{propertyId}")]
    public async Task<IActionResult> GetPropertyStatistics(Guid propertyId)
    {
        var stats = await _statisticsService.GetPropertyStatisticsAsync(propertyId);
        if (stats == null)
            return NotFound(new { message = "Property statistics not found" });
        
        return Ok(stats);
    }

    /// <summary>
    /// Get statistics for all properties
    /// </summary>
    [HttpGet("properties")]
    public async Task<IActionResult> GetAllPropertyStatistics()
    {
        var stats = await _statisticsService.GetAllPropertyStatisticsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Get statistics for a specific rental
    /// </summary>
    [HttpGet("rentals/{rentalId}")]
    public async Task<IActionResult> GetRentalStatistics(Guid rentalId)
    {
        var stats = await _statisticsService.GetRentalStatisticsAsync(rentalId);
        if (stats == null)
            return NotFound(new { message = "Rental statistics not found" });
        
        return Ok(stats);
    }

    /// <summary>
    /// Get statistics for all rentals
    /// </summary>
    [HttpGet("rentals")]
    public async Task<IActionResult> GetAllRentalStatistics()
    {
        var stats = await _statisticsService.GetAllRentalStatisticsAsync();
        return Ok(stats);
    }

    /// <summary>
    /// Refresh statistics for a specific user
    /// </summary>
    [HttpPost("users/{userId}/refresh")]
    public async Task<IActionResult> RefreshUserStats(Guid userId)
    {
        await _statisticsService.RefreshUserOwnershipStatsAsync(userId);
        return Ok(new { message = "User statistics refreshed successfully" });
    }

    /// <summary>
    /// Refresh statistics for a specific property
    /// </summary>
    [HttpPost("properties/{propertyId}/refresh")]
    public async Task<IActionResult> RefreshPropertyStats(Guid propertyId)
    {
        await _statisticsService.RefreshPropertyStatisticsAsync(propertyId);
        return Ok(new { message = "Property statistics refreshed successfully" });
    }

    /// <summary>
    /// Refresh statistics for a specific rental
    /// </summary>
    [HttpPost("rentals/{rentalId}/refresh")]
    public async Task<IActionResult> RefreshRentalStats(Guid rentalId)
    {
        await _statisticsService.RefreshRentalStatisticsAsync(rentalId);
        return Ok(new { message = "Rental statistics refreshed successfully" });
    }

    /// <summary>
    /// Refresh all statistics (users, properties, rentals)
    /// </summary>
    [HttpPost("refresh-all")]
    public async Task<IActionResult> RefreshAllStatistics()
    {
        await _statisticsService.RefreshAllStatisticsAsync();
        return Ok(new { message = "All statistics refreshed successfully" });
    }
}
