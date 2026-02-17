using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;
using System.Security.Claims;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a summarized overview of the dashboard data.
    /// </summary>
    /// <returns>Dashboard overview statistics.</returns>
    [HttpGet("overview")]
    public async Task<IActionResult> GetOverview()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var overview = await _dashboardService.GetDashboardOverviewAsync(userId, isAdmin);
        return Ok(overview);
    }

    /// <summary>
    /// Performs a global search across multiple entities.
    /// </summary>
    /// <param name="searchDto">The search criteria.</param>
    /// <returns>Search results.</returns>
    [HttpPost("search")]
    public async Task<IActionResult> GlobalSearch([FromBody] GlobalSearchDto searchDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var results = await _dashboardService.GlobalSearchAsync(searchDto, userId, isAdmin);
        return Ok(results);
    }
}
