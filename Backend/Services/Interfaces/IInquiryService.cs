using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IInquiryService
{
    Task<(List<RentalInquiryDto> Inquiries, int TotalCount)> GetInquiriesAsync(InquiryFilterDto filter, int? userId = null, bool isAdmin = false);
    Task<RentalInquiryDto?> GetInquiryByIdAsync(int id, int? userId = null, bool isAdmin = false);
    Task<RentalInquiryDto> CreateInquiryAsync(InquiryCreateDto createDto, int? userId);
    Task<RentalInquiryDto?> UpdateInquiryAsync(InquiryUpdateDto updateDto, int userId, bool isAdmin = false);
    Task<bool> DeleteInquiryAsync(int id, int userId, bool isAdmin = false);
}
