using Microsoft.AspNetCore.Mvc;
using MiniRent.Backend.Services.Interfaces;
using MiniRent.Backend.Services;

namespace MiniRent.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OwnershipController : ControllerBase
{
    private readonly IOwnershipService _ownershipService;
    private readonly OwnershipTableService _tableService;

    public OwnershipController(IOwnershipService ownershipService, OwnershipTableService tableService)
    {
        _ownershipService = ownershipService;
        _tableService = tableService;
    }

    /// <summary>
    /// Check if UserPropertyOwnership table exists and get status
    /// </summary>
    [HttpGet("table/status")]
    public async Task<IActionResult> GetTableStatus()
    {
        var (tableExists, recordCount) = await _tableService.GetTableStatusAsync();
        
        // Also check how many properties exist
        var propertyCount = await _tableService.GetAvailablePropertiesCountAsync();
        
        return Ok(new 
        { 
            tableExists, 
            recordCount,
            availableProperties = propertyCount,
            message = tableExists 
                ? recordCount > 0
                    ? $"Table exists with {recordCount} records. {propertyCount} properties available."
                    : $"Table exists but is EMPTY. {propertyCount} properties available. Run POST /api/ownership/table/populate to add data."
                : "Table does not exist. Run the SQL script: Backend/create-ownership-table.sql"
        });
    }

    /// <summary>
    /// Populate UserPropertyOwnership table with existing data
    /// </summary>
    [HttpPost("table/populate")]
    public async Task<IActionResult> PopulateTable()
    {
        try
        {
            var addedCount = await _tableService.PopulateOwnershipTableAsync();
            return Ok(new 
            { 
                success = true,
                recordsAdded = addedCount,
                message = $"Successfully added {addedCount} ownership records"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new 
            { 
                success = false,
                error = ex.Message,
                message = "Failed to populate table. The table may not exist. Run the SQL script: Backend/create-ownership-table.sql"
            });
        }
    }

    /// <summary>
    /// Add ownership record for a property
    /// </summary>
    [HttpPost("table/add")]
    public async Task<IActionResult> AddOwnershipRecord([FromBody] AddOwnershipRequest request)
    {
        var ownership = await _tableService.AddOwnershipRecordAsync(request.UserId, request.PropertyId, request.Notes);
        if (ownership == null)
            return BadRequest(new { message = "Failed to add ownership record. User or property may not exist." });
        
        return Ok(ownership);
    }

    /// <summary>
    /// Get all user-property ownership relationships with details
    /// </summary>
    [HttpGet("all")]
    public async Task<IActionResult> GetAllOwnerships()
    {
        var ownerships = await _ownershipService.GetAllUserPropertyOwnershipsAsync();
        return Ok(ownerships);
    }

    /// <summary>
    /// Get all properties owned by a specific user
    /// </summary>
    [HttpGet("users/{userId}/properties")]
    public async Task<IActionResult> GetPropertiesByOwner(Guid userId)
    {
        var properties = await _ownershipService.GetPropertiesByOwnerAsync(userId);
        return Ok(properties);
    }

    /// <summary>
    /// Get ownership details for a specific property
    /// </summary>
    [HttpGet("properties/{propertyId}")]
    public async Task<IActionResult> GetPropertyOwnership(Guid propertyId)
    {
        var ownership = await _ownershipService.GetPropertyOwnershipAsync(propertyId);
        if (ownership == null)
            return NotFound(new { message = "Property ownership not found" });
        
        return Ok(ownership);
    }

    /// <summary>
    /// Get summary of properties for all users
    /// </summary>
    [HttpGet("summaries")]
    public async Task<IActionResult> GetAllUserSummaries()
    {
        var summaries = await _ownershipService.GetAllUserPropertiesSummariesAsync();
        return Ok(summaries);
    }

    /// <summary>
    /// Get property summary for a specific user
    /// </summary>
    [HttpGet("users/{userId}/summary")]
    public async Task<IActionResult> GetUserSummary(Guid userId)
    {
        var summary = await _ownershipService.GetUserPropertiesSummaryAsync(userId);
        if (summary == null)
            return NotFound(new { message = "User summary not found" });
        
        return Ok(summary);
    }

    /// <summary>
    /// Get ownership history for all properties
    /// </summary>
    [HttpGet("history")]
    public async Task<IActionResult> GetAllOwnershipHistory()
    {
        var histories = await _ownershipService.GetAllPropertyOwnershipHistoriesAsync();
        return Ok(histories);
    }

    /// <summary>
    /// Get ownership history for a specific property
    /// </summary>
    [HttpGet("properties/{propertyId}/history")]
    public async Task<IActionResult> GetPropertyHistory(Guid propertyId)
    {
        var history = await _ownershipService.GetPropertyOwnershipHistoryAsync(propertyId);
        if (history == null)
            return NotFound(new { message = "Property history not found" });
        
        return Ok(history);
    }

    /// <summary>
    /// Get simple list of properties for a user
    /// </summary>
    [HttpGet("users/{userId}/properties/simple")]
    public async Task<IActionResult> GetUserPropertiesSimple(Guid userId)
    {
        var properties = await _ownershipService.GetUserPropertiesSimpleAsync(userId);
        return Ok(properties);
    }

    /// <summary>
    /// Get count of properties owned by a user
    /// </summary>
    [HttpGet("users/{userId}/count")]
    public async Task<IActionResult> GetPropertyCount(Guid userId)
    {
        var count = await _ownershipService.GetPropertyCountByOwnerAsync(userId);
        return Ok(new { userId, propertyCount = count });
    }
}

public class AddOwnershipRequest
{
    public Guid UserId { get; set; }
    public Guid PropertyId { get; set; }
    public string? Notes { get; set; }
}
