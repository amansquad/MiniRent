using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class PropertyService : IPropertyService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public PropertyService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(List<PropertyDto> Properties, int TotalCount)> GetPropertiesAsync(PropertyFilterDto filter, int? userId = null, bool isAdmin = false)
    {
        var query = _context.Properties
            .Include(p => p.CreatedBy)
            .Include(p => p.RentalHistory)
            .Where(p => !p.IsDeleted);

        // If not admin, filter by user's own properties
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(p => p.CreatedById == userId.Value);
        }

        // Apply filters
        if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<PropertyStatus>(filter.Status, true, out var status))
        {
            query = query.Where(p => p.Status == status);
        }

        if (filter.MinBedrooms.HasValue)
        {
            query = query.Where(p => p.Bedrooms >= filter.MinBedrooms.Value);
        }

        if (filter.MaxBedrooms.HasValue)
        {
            query = query.Where(p => p.Bedrooms <= filter.MaxBedrooms.Value);
        }

        if (filter.MinRent.HasValue)
        {
            query = query.Where(p => p.MonthlyRent >= filter.MinRent.Value);
        }

        if (filter.MaxRent.HasValue)
        {
            query = query.Where(p => p.MonthlyRent <= filter.MaxRent.Value);
        }

        if (!string.IsNullOrEmpty(filter.SearchAddress))
        {
            query = query.Where(p => p.Address.Contains(filter.SearchAddress));
        }

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "rent" => filter.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.MonthlyRent)
                : query.OrderBy(p => p.MonthlyRent),
            _ => filter.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync();

        var properties = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (_mapper.Map<List<PropertyDto>>(properties), totalCount);
    }

    public async Task<PropertyDto?> GetPropertyByIdAsync(int id, int? userId = null, bool isAdmin = false)
    {
        var query = _context.Properties
            .Include(p => p.CreatedBy)
            .Include(p => p.RentalHistory)
                .ThenInclude(r => r.CreatedBy)
            .Where(p => p.Id == id && !p.IsDeleted);

        // If not admin, filter by user's own properties
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(p => p.CreatedById == userId.Value);
        }

        var property = await query.FirstOrDefaultAsync();

        return property != null ? _mapper.Map<PropertyDto>(property) : null;
    }

    public async Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto createDto, int userId)
    {
        var property = _mapper.Map<Property>(createDto);
        property.CreatedAt = DateTime.UtcNow;
        property.CreatedById = userId;

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();

        // Reload with related data
        var createdProperty = await _context.Properties
            .Include(p => p.CreatedBy)
            .Include(p => p.RentalHistory)
            .FirstAsync(p => p.Id == property.Id);

        return _mapper.Map<PropertyDto>(createdProperty);
    }

    public async Task<PropertyDto?> UpdatePropertyAsync(PropertyUpdateDto updateDto, int userId, bool isAdmin = false)
    {
        var query = _context.Properties
            .Where(p => p.Id == updateDto.Id && !p.IsDeleted);

        // If not admin, only allow updating own properties
        if (!isAdmin)
        {
            query = query.Where(p => p.CreatedById == userId);
        }

        var property = await query.FirstOrDefaultAsync();

        if (property == null)
            return null;

        _mapper.Map(updateDto, property);
        property.UpdatedAt = DateTime.UtcNow;
        property.UpdatedById = userId;

        await _context.SaveChangesAsync();

        // Reload with related data
        var updatedProperty = await _context.Properties
            .Include(p => p.CreatedBy)
            .Include(p => p.UpdatedBy)
            .Include(p => p.RentalHistory)
            .FirstAsync(p => p.Id == property.Id);

        return _mapper.Map<PropertyDto>(updatedProperty);
    }

    public async Task<bool> DeletePropertyAsync(int id, int userId, bool isAdmin = false)
    {
        var query = _context.Properties
            .Where(p => p.Id == id && !p.IsDeleted);

        // If not admin, only allow deleting own properties
        if (!isAdmin)
        {
            query = query.Where(p => p.CreatedById == userId);
        }

        var property = await query.FirstOrDefaultAsync();

        if (property == null)
            return false;

        property.IsDeleted = true;
        property.UpdatedAt = DateTime.UtcNow;
        property.UpdatedById = userId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PropertyDto?> UpdatePropertyStatusAsync(int id, PropertyStatusUpdateDto statusDto, int userId, bool isAdmin = false)
    {
        var query = _context.Properties
            .Where(p => p.Id == id && !p.IsDeleted);

        // If not admin, only allow updating own properties
        if (!isAdmin)
        {
            query = query.Where(p => p.CreatedById == userId);
        }

        var property = await query.FirstOrDefaultAsync();

        if (property == null || !Enum.TryParse<PropertyStatus>(statusDto.Status, true, out var newStatus))
            return null;

        property.Status = newStatus;
        property.UpdatedAt = DateTime.UtcNow;
        property.UpdatedById = userId;

        await _context.SaveChangesAsync();

        // Reload with related data
        var updatedProperty = await _context.Properties
            .Include(p => p.CreatedBy)
            .Include(p => p.UpdatedBy)
            .Include(p => p.RentalHistory)
            .FirstAsync(p => p.Id == property.Id);

        return _mapper.Map<PropertyDto>(updatedProperty);
    }
}
