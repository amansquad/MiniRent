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

    public RentalsController(IRentalService rentalService)
    {
        _rentalService = rentalService;
    }

    [HttpGet]
    public async Task<IActionResult> GetRentals([FromQuery] RentalFilterDto filter, [FromQuery] string? mode = null)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        // If mode is "my", filter by userId. Otherwise, show all (marketplace view).
        int? filterUserId = (mode == "my" && userId.HasValue) ? userId : null;

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
    public async Task<IActionResult> GetRental(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var rental = await _rentalService.GetRentalByIdAsync(id, userId, isAdmin);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRental([FromBody] RentalCreateDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var rental = await _rentalService.CreateRentalAsync(createDto, userId);

        return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, rental);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRental(int id, [FromBody] RentalUpdateDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
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
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] RentalUpdateDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
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
    public async Task<IActionResult> EndRental(int id, [FromBody] RentalEndDto endDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        bool isAdmin = roleClaim?.Value == "Admin";

        var rental = await _rentalService.EndRentalAsync(id, endDto, userId, isAdmin);

        if (rental == null)
        {
            return NotFound();
        }

        return Ok(rental);
    }
}
