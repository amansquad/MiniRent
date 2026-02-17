using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IInquiryService
{
    Task<(List<RentalInquiryDto> Inquiries, int TotalCount)> GetInquiriesAsync(InquiryFilterDto filter, Guid? userId = null, bool isAdmin = false);
    Task<RentalInquiryDto?> GetInquiryByIdAsync(Guid id, Guid? userId = null, bool isAdmin = false);
    Task<RentalInquiryDto> CreateInquiryAsync(InquiryCreateDto createDto, Guid? userId);
    Task<RentalInquiryDto?> UpdateInquiryAsync(InquiryUpdateDto updateDto, Guid userId, bool isAdmin = false);
    Task<bool> DeleteInquiryAsync(Guid id, Guid userId, bool isAdmin = false);
}
