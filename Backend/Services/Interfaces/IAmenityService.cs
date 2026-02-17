using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IAmenityService
{
    Task<List<AmenityDto>> GetAllAmenitiesAsync();
    Task<AmenityDto> CreateAmenityAsync(AmenityCreateDto createDto);
    Task<bool> AddAmenityToPropertyAsync(Guid propertyId, Guid amenityId);
    Task<bool> RemoveAmenityFromPropertyAsync(Guid propertyId, Guid amenityId);
}
