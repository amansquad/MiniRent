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
    private readonly ILogger<PropertiesController> _logger;

    public PropertiesController(IPropertyService propertyService, ILogger<PropertiesController> logger)
    {
        _propertyService = propertyService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of properties with optional filtering.
    /// </summary>
    /// <param name="status">Filter by property status (Available, Rented, etc.).</param>
    /// <param name="minBedrooms">Minimum number of bedrooms.</param>
    /// <param name="maxBedrooms">Maximum number of bedrooms.</param>
    /// <param name="minRent">Minimum monthly rent.</param>
    /// <param name="maxRent">Maximum monthly rent.</param>
    /// <param name="searchAddress">Search address by keyword.</param>
    /// <param name="sortBy">Field to sort by (rent, date).</param>
    /// <param name="sortOrder">Sort order (asc, desc).</param>
    /// <param name="page">Page number.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="mode">"my" for user's owned properties, null for all.</param>
    /// <returns>A paginated list of properties.</returns>
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
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        // If mode is "my", filter by userId. Otherwise, show all (marketplace view).
        Guid? filterUserId = (mode == "my" && userId.HasValue) ? userId : null;

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

    /// <summary>
    /// Gets a specific property by its unique identifier.
    /// </summary>
    /// <param name="id">The property GUID.</param>
    /// <returns>The property details if found.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProperty(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var property = await _propertyService.GetPropertyByIdAsync(id, userId, isAdmin);

        if (property == null)
        {
            return NotFound();
        }

        return Ok(property);
    }

    /// <summary>
    /// Creates a new property listing.
    /// </summary>
    /// <param name="createDto">The property details.</param>
    /// <returns>The created property.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateProperty([FromBody] PropertyCreateDto createDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        _logger.LogInformation("Creating new property: {Title} by User: {UserId}", createDto.Title, userId);
        
        var property = await _propertyService.CreatePropertyAsync(createDto, userId);

        _logger.LogInformation("Property created successfully: {Id}", property.Id);
        return CreatedAtAction(nameof(GetProperty), new { id = property.Id }, property);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProperty(Guid id, [FromBody] PropertyUpdateDto updateDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
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

    /// <summary>
    /// Deletes a property listing. Only owners or admins can delete.
    /// </summary>
    /// <param name="id">The property GUID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProperty(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role) ?? User.FindFirst("role");
        bool isAdmin = roleClaim?.Value == "Admin";

        _logger.LogInformation("Attempting to delete property: {Id} by User: {UserId}", id, userId);
        
        var success = await _propertyService.DeletePropertyAsync(id, userId, isAdmin);

        if (!success)
        {
            _logger.LogWarning("Delete failed for property: {Id}. Not found or unauthorized.", id);
            return NotFound(new { message = "Property not found or unauthorized" });
        }

        _logger.LogInformation("Property deleted successfully: {Id}", id);
        return NoContent();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdatePropertyStatus(Guid id, [FromBody] PropertyStatusUpdateDto statusDto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
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
