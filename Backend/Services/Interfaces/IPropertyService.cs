using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IPropertyService
{
    Task<(List<PropertyDto> Properties, int TotalCount)> GetPropertiesAsync(PropertyFilterDto filter, int? userId = null, bool isAdmin = false);
    Task<PropertyDto?> GetPropertyByIdAsync(int id, int? userId = null, bool isAdmin = false);
    Task<PropertyDto> CreatePropertyAsync(PropertyCreateDto createDto, int userId);
    Task<PropertyDto?> UpdatePropertyAsync(PropertyUpdateDto updateDto, int userId, bool isAdmin = false);
    Task<bool> DeletePropertyAsync(int id, int userId, bool isAdmin = false);
    Task<PropertyDto?> UpdatePropertyStatusAsync(int id, PropertyStatusUpdateDto statusDto, int userId, bool isAdmin = false);
}
