using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;
using System.Security.Claims;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProperties(
        [FromQuery] string? status,
        [FromQuery] int? minBedrooms,
        [FromQuery] int? maxBedrooms,
        [FromQuery] decimal? minRent,
        [FromQuery] decimal? maxRent,
        [FromQuery] string? searchAddress,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? mode = null)
    {
        var filter = new PropertyFilterDto
        {
            Status = status,
            MinBedrooms = minBedrooms,
            MaxBedrooms = maxBedrooms,
            MinRent = minRent,
            MaxRent = maxRent,
            SearchAddress = searchAddress,
            SortBy = sortBy,
            SortOrder = sortOrder,
            Page = page,
            PageSize = pageSize
        };

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        // If mode is "my", filter by userId. Otherwise, show all (marketplace view).
        int? filterUserId = (mode == "my" && userId.HasValue) ? userId : null;

        var (properties, totalCount) = await _propertyService.GetPropertiesAsync(filter, filterUserId, isAdmin);

        return Ok(new
        {
            data = properties,
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
    public async Task<IActionResult> GetProperty(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var property = await _propertyService.GetPropertyByIdAsync(id, userId, isAdmin);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] PropertyCreateDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var property = await _propertyService.CreatePropertyAsync(createDto, userId);

        return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(int id, [FromBody] PropertyUpdateDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        bool isAdmin = roleClaim?.Value == "Admin";
        updateDto.Id = id;
        var property = await _propertyService.UpdatePropertyAsync(updateDto, userId, isAdmin);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        bool isAdmin = roleClaim?.Value == "Admin";

        var success = await _propertyService.DeletePropertyAsync(id, userId, isAdmin);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdatePropertyStatus(int id, [FromBody] PropertyStatusUpdateDto statusDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        bool isAdmin = roleClaim?.Value == "Admin";

        var property = await _propertyService.UpdatePropertyStatusAsync(id, statusDto, userId, isAdmin);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }
}
