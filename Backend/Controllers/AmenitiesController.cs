using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AmenitiesController : ControllerBase
{
    private readonly IAmenityService _amenityService;
    private readonly ILogger<AmenitiesController> _logger;

    public AmenitiesController(IAmenityService amenityService, ILogger<AmenitiesController> logger)
    {
        _amenityService = amenityService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of all available amenities.
    /// </summary>
    /// <returns>A list of amenities.</returns>
    [HttpGet]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<ActionResult<List<AmenityDto>>> GetAmenities()
    {
        return Ok(await _amenityService.GetAllAmenitiesAsync());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AmenityDto>> CreateAmenity(AmenityCreateDto createDto)
    {
        try
        {
            var amenity = await _amenityService.CreateAmenityAsync(createDto);
            return CreatedAtAction(nameof(GetAmenities), new { id = amenity.Id }, amenity);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("property/{propertyId}/amenity/{amenityId}")]
    [Authorize]
    public async Task<IActionResult> AddAmenityToProperty(Guid propertyId, Guid amenityId)
    {
        var success = await _amenityService.AddAmenityToPropertyAsync(propertyId, amenityId);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("property/{propertyId}/amenity/{amenityId}")]
    [Authorize]
    public async Task<IActionResult> RemoveAmenityFromProperty(Guid propertyId, Guid amenityId)
    {
        var success = await _amenityService.RemoveAmenityFromPropertyAsync(propertyId, amenityId);
        return success ? Ok() : NotFound();
    }
}
