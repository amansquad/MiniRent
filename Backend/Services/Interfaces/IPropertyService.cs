using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IPropertyService
{
    Task<(List<PropertyDto> Properties, int TotalCount)> GetPropertiesAsync(PropertyFilterDto filter, Guid? userId = null, bool isAdmin = false);
    Task<PropertyDto?> GetPropertyByIdAsync(Guid id, Guid? userId = null, bool isAdmin = false);
    Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto createDto, Guid userId);
    Task<PropertyDto?> UpdatePropertyAsync(PropertyUpdateDto updateDto, Guid userId, bool isAdmin = false);
    Task<bool> DeletePropertyAsync(Guid id, Guid userId, bool isAdmin = false);
    Task<PropertyDto?> UpdatePropertyStatusAsync(Guid id, PropertyStatusUpdateDto statusDto, Guid userId, bool isAdmin = false);
}
