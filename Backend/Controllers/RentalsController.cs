using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;
using System.Security.Claims;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RentalsController : ControllerBase
{
    private readonly IRentalService _rentalService;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(IRentalService rentalService, ILogger<RentalsController> logger)
    {
        _rentalService = rentalService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of rental records.
    /// </summary>
    /// <param name="filter">The rental filter criteria.</param>
    /// <param name="mode">"my" for user's rentals, null for all.</param>
    /// <returns>A paginated list of rentals.</returns>
    [HttpGet]
    public async Task<IActionResult> GetRentals([FromQuery] RentalFilterDto filter, [FromQuery] string? mode = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        // If mode is "my", filter by userId. Otherwise, show all (marketplace view).
        Guid? filterUserId = (mode == "my" && userId.HasValue) ? userId : null;

        var (rentals, totalCount) = await _rentalService.GetRentalsAsync(filter, filterUserId, isAdmin);

        return Ok(new
        {
            data = rentals,
            pagination = new
            {
                page = filter.Page,
                pageSize = filter.PageSize,
                totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRental(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var rental = await _rentalService.GetRentalByIdAsync(id, userId, isAdmin);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    /// <summary>
    /// Creates a new rental record.
    /// </summary>
    /// <param name="createDto">The rental details.</param>
    /// <returns>The created rental record.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateRental([FromBody] RentalCreateDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        _logger.LogInformation("Creating new rental for Property: {PropertyId} by User: {UserId}", createDto.PropertyId, userId);
        
        var rental = await _rentalService.CreateRentalAsync(createDto, userId);

        _logger.LogInformation("Rental created successfully: {Id}", rental.Id);
        return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRental(Guid id, [FromBody] RentalUpdateDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        bool isAdmin = roleClaim?.Value == "Admin";
        updateDto.Id = id;

        var rental = await _rentalService.UpdateRentalAsync(updateDto, userId, isAdmin);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] RentalUpdateDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        bool isAdmin = roleClaim?.Value == "Admin";
        
        var status = updateDto.Status;
        if (string.IsNullOrEmpty(status))
        {
            return BadRequest("Status is required");
        }

        var rental = await _rentalService.UpdateStatusAsync(id, status, userId, isAdmin);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    [HttpPost("{id}/end")]
    public async Task<IActionResult> EndRental(Guid id, [FromBody] RentalEndDto endDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        bool isAdmin = roleClaim?.Value == "Admin";

        var rental = await _rentalService.EndRentalAsync(id, endDto, userId, isAdmin);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    /// <summary>
    /// Deletes a rental record.
    /// </summary>
    /// <param name="id">The rental GUID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRental(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        bool isAdmin = roleClaim?.Value == "Admin";

        _logger.LogInformation("Attempting to delete rental: {Id} by User: {UserId}", id, userId);
        
        var success = await _rentalService.DeleteRentalAsync(id, userId, isAdmin);

        if (!success)
        {
            _logger.LogWarning("Delete failed for rental: {Id}. Not found or unauthorized.", id);
            return BadRequest(new { message = "Rental could not be deleted. It may be active or not found." });
        }

        _logger.LogInformation("Rental deleted successfully: {Id}", id);
        return NoContent();
    }
}
