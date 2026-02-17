using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IRentalService
{
    Task<(List<RentalRecordDto> Rentals, int TotalCount)> GetRentalsAsync(RentalFilterDto filter, Guid? userId = null, bool isAdmin = false);
    Task<RentalRecordDto?> GetRentalByIdAsync(Guid id, Guid? userId = null, bool isAdmin = false);
    Task<RentalRecordDto> CreateRentalAsync(RentalCreateDto createDto, Guid userId);
    Task<RentalRecordDto?> UpdateRentalAsync(RentalUpdateDto updateDto, Guid userId, bool isAdmin = false);
    Task<RentalRecordDto?> UpdateStatusAsync(Guid id, string status, Guid userId, bool isAdmin = false);
    Task<RentalRecordDto?> EndRentalAsync(Guid id, RentalEndDto endDto, Guid userId, bool isAdmin = false);
    Task<bool> DeleteRentalAsync(Guid id, Guid userId, bool isAdmin = false);
}
