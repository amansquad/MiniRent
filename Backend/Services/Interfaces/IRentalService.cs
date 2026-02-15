using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IRentalService
{
    Task<(List<RentalRecordDto> Rentals, int TotalCount)> GetRentalsAsync(RentalFilterDto filter, int? userId = null, bool isAdmin = false);
    Task<RentalRecordDto?> GetRentalByIdAsync(int id, int? userId = null, bool isAdmin = false);
    Task<RentalRecordDto> CreateRentalAsync(RentalCreateDto createDto, int userId);
    Task<RentalRecordDto?> UpdateRentalAsync(RentalUpdateDto updateDto, int userId, bool isAdmin = false);
    Task<RentalRecordDto?> UpdateStatusAsync(int id, string status, int userId, bool isAdmin = false);
    Task<RentalRecordDto?> EndRentalAsync(int id, RentalEndDto endDto, int userId, bool isAdmin = false);
    Task<bool> DeleteRentalAsync(int id, int userId, bool isAdmin = false);
}
