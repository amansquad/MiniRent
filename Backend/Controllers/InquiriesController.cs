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

    public InquiriesController(IInquiryService inquiryService)
    {
        _inquiryService = inquiryService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetInquiries([FromQuery] InquiryFilterDto filter)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
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
    public async Task<IActionResult> GetInquiry(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        int? userId = userIdClaim != null ? int.Parse(userIdClaim.Value) : null;
        bool isAdmin = roleClaim?.Value == "Admin";

        var inquiry = await _inquiryService.GetInquiryByIdAsync(id, userId, isAdmin);

        if (inquiry == null)
        {
            return NotFound();
        }

        return Ok(inquiry);
    }

    [HttpPost]
    public async Task<IActionResult> CreateInquiry([FromBody] InquiryCreateDto createDto)
    {
        int? userId = null;

        // Get user ID if authenticated
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null)
        {
            userId = int.Parse(userIdClaim.Value);
        }

        var inquiry = await _inquiryService.CreateInquiryAsync(createDto, userId);

        return CreatedAtAction(nameof(GetInquiry), new { id = inquiry.Id }, inquiry);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateInquiry(int id, [FromBody] InquiryUpdateDto updateDto)
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

        var inquiry = await _inquiryService.UpdateInquiryAsync(updateDto, userId, isAdmin);

        if (inquiry == null)
        {
            return NotFound();
        }

        return Ok(inquiry);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteInquiry(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
        {
            return Unauthorized();
        }

        var userId = int.Parse(userIdClaim.Value);
        var roleClaim = User.FindFirst(ClaimTypes.Role);
        bool isAdmin = roleClaim?.Value == "Admin";

        var success = await _inquiryService.DeleteInquiryAsync(id, userId, isAdmin);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
