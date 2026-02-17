using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Services.Interfaces;
using System.Security.Claims;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InquiriesController : ControllerBase
{
    private readonly IInquiryService _inquiryService;
    private readonly ILogger<InquiriesController> _logger;

    public InquiriesController(IInquiryService inquiryService, ILogger<InquiriesController> logger)
    {
        _inquiryService = inquiryService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of rental inquiries.
    /// </summary>
    /// <param name="filter">The inquiry filter criteria.</param>
    /// <returns>A paginated list of inquiries.</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetInquiries([FromQuery] InquiryFilterDto filter)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var (inquiries, totalCount) = await _inquiryService.GetInquiriesAsync(filter, userId, isAdmin);

        return Ok(new
        {
            data = inquiries,
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
    [Authorize]
    public async Task<IActionResult> GetInquiry(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        Guid? userId = userIdClaim != null ? Guid.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var inquiry = await _inquiryService.GetInquiryByIdAsync(id, userId, isAdmin);

        if (inquiry == null)
        {
            return NotFound();
        }

        return Ok(inquiry);
    }

    /// <summary>
    /// Creates a new rental inquiry for a property.
    /// </summary>
    /// <param name="createDto">The inquiry details.</param>
    /// <returns>The created inquiry.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateInquiry([FromBody] InquiryCreateDto createDto)
    {
        _logger.LogInformation("Creating new inquiry for Property: {PropertyId}", createDto.PropertyId);
        
        Guid? userId = null;
        // Get user ID if authenticated
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            userId = Guid.Parse(userIdClaim.Value);
        }

        var inquiry = await _inquiryService.CreateInquiryAsync(createDto, userId);

        _logger.LogInformation("Inquiry created successfully: {Id}", inquiry.Id);
        return CreatedAtAction(nameof(GetInquiry), new { id = inquiry.Id }, inquiry);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateInquiry(Guid id, [FromBody] InquiryUpdateDto updateDto)
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

        var inquiry = await _inquiryService.UpdateInquiryAsync(updateDto, userId, isAdmin);

        if (inquiry == null)
        {
            return NotFound();
        }

        return Ok(inquiry);
    }

    /// <summary>
    /// Deletes an inquiry.
    /// </summary>
    /// <param name="id">The inquiry GUID.</param>
    /// <returns>No content if successful.</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteInquiry(Guid id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = Guid.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        bool isAdmin = roleClaim?.Value == "Admin";

        _logger.LogInformation("Attempting to delete inquiry: {Id} by User: {UserId}", id, userId);
        
        var success = await _inquiryService.DeleteInquiryAsync(id, userId, isAdmin);

        if (!success)
        {
            _logger.LogWarning("Delete failed for inquiry: {Id}. Not found or unauthorized.", id);
            return NotFound(new { message = "Inquiry not found or unauthorized" });
        }

        _logger.LogInformation("Inquiry deleted successfully: {Id}", id);
        return NoContent();
    }
}
