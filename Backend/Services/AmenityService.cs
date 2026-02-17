using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class AmenityService : IAmenityService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AmenityService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<AmenityDto>> GetAllAmenitiesAsync()
    {
        var amenities = await _context.Amenities.OrderBy(a => a.Name).ToListAsync();
        return _mapper.Map<List<AmenityDto>>(amenities);
    }

    public async Task<AmenityDto> CreateAmenityAsync(AmenityCreateDto createDto)
    {
        var existing = await _context.Amenities.AnyAsync(a => a.Name == createDto.Name);
        if (existing)
        {
            throw new ArgumentException($"Amenity with name '{createDto.Name}' already exists.");
        }

        var amenity = _mapper.Map<Amenity>(createDto);
        _context.Amenities.Add(amenity);
        await _context.SaveChangesAsync();

        return _mapper.Map<AmenityDto>(amenity);
    }

    public async Task<bool> AddAmenityToPropertyAsync(Guid propertyId, Guid amenityId)
    {
        var property = await _context.Properties
            .Include(p => p.Amenities)
            .FirstOrDefaultAsync(p => p.Id == propertyId && !p.IsDeleted);

        if (property == null) return false;

        var amenity = await _context.Amenities.FindAsync(amenityId);
        if (amenity == null) return false;

        if (!property.Amenities.Any(a => a.Id == amenityId))
        {
            property.Amenities.Add(amenity);
            await _context.SaveChangesAsync();
        }

        return true;
    }

    public async Task<bool> RemoveAmenityFromPropertyAsync(Guid propertyId, Guid amenityId)
    {
        var property = await _context.Properties
            .Include(p => p.Amenities)
            .FirstOrDefaultAsync(p => p.Id == propertyId && !p.IsDeleted);

        if (property == null) return false;

        var amenity = property.Amenities.FirstOrDefault(a => a.Id == amenityId);
        if (amenity != null)
        {
            property.Amenities.Remove(amenity);
            await _context.SaveChangesAsync();
        }

        return true;
    }
}
