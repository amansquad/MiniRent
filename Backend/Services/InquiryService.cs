using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class InquiryService : IInquiryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public InquiryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(List<RentalInquiryDto> Inquiries, int TotalCount)> GetInquiriesAsync(InquiryFilterDto filter, Guid? userId = null, bool isAdmin = false)
    {
        var query = _context.RentalInquiries
            .Include(i => i.Property)
            .Include(i => i.CreatedBy)
            .Include(i => i.RentalRecord)
            .AsQueryable();

        // If not admin, filter by user's own inquiries OR inquiries for user's properties
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(i => i.CreatedById == userId.Value || (i.Property != null && i.Property.CreatedById == userId.Value));
        }

        // Apply filters
        if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<InquiryStatus>(filter.Status, true, out var status))
        {
            query = query.Where(i => i.Status == status);
        }

        if (filter.DateFrom.HasValue)
        {
            query = query.Where(i => i.CreatedAt >= filter.DateFrom.Value);
        }

        if (filter.DateTo.HasValue)
        {
            query = query.Where(i => i.CreatedAt <= filter.DateTo.Value);
        }

        if (filter.PropertyId.HasValue)
        {
            query = query.Where(i => i.PropertyId == filter.PropertyId.Value);
        }

        if (!string.IsNullOrEmpty(filter.SearchName))
        {
            query = query.Where(i => i.Name.Contains(filter.SearchName) ||
                                   i.Email.Contains(filter.SearchName));
        }

        var totalCount = await query.CountAsync();

        var inquiries = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (_mapper.Map<List<RentalInquiryDto>>(inquiries), totalCount);
    }

    public async Task<RentalInquiryDto?> GetInquiryByIdAsync(Guid id, Guid? userId = null, bool isAdmin = false)
    {
        var query = _context.RentalInquiries
            .Include(i => i.Property)
            .Include(i => i.CreatedBy)
            .Include(i => i.RentalRecord)
            .Where(i => i.Id == id);

        // If not admin, filter by user's own inquiries OR inquiries for user's properties
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(i => i.CreatedById == userId.Value || (i.Property != null && i.Property.CreatedById == userId.Value));
        }

        var inquiry = await query.FirstOrDefaultAsync();

        return inquiry != null ? _mapper.Map<RentalInquiryDto>(inquiry) : null;
    }

    public async Task<RentalInquiryDto> CreateInquiryAsync(InquiryCreateDto createDto, Guid? userId)
    {
        // Validate property exists if provided
        if (createDto.PropertyId.HasValue)
        {
            var propertyExists = await _context.Properties
                .AnyAsync(p => p.Id == createDto.PropertyId.Value && !p.IsDeleted);

            if (!propertyExists)
            {
                throw new ArgumentException("Property not found");
            }
        }

        var inquiry = _mapper.Map<RentalInquiry>(createDto);
        inquiry.CreatedAt = DateTime.UtcNow;
        inquiry.CreatedById = userId;

        _context.RentalInquiries.Add(inquiry);
        await _context.SaveChangesAsync();

        // Reload with related data
        var createdInquiry = await _context.RentalInquiries
            .Include(i => i.Property)
            .Include(i => i.CreatedBy)
            .Include(i => i.RentalRecord)
            .FirstAsync(i => i.Id == inquiry.Id);

        return _mapper.Map<RentalInquiryDto>(createdInquiry);
    }

    public async Task<RentalInquiryDto?> UpdateInquiryAsync(InquiryUpdateDto updateDto, Guid userId, bool isAdmin = false)
    {
        var query = _context.RentalInquiries
            .Include(i => i.Property)
            .Where(i => i.Id == updateDto.Id);

        // If not admin, only allow updating own inquiries OR inquiries for own properties
        if (!isAdmin)
        {
            query = query.Where(i => i.CreatedById == userId || (i.Property != null && i.Property.CreatedById == userId));
        }

        var inquiry = await query.FirstOrDefaultAsync();

        if (inquiry == null)
            return null;

        if (!Enum.TryParse<InquiryStatus>(updateDto.Status, true, out var newStatus))
        {
            throw new ArgumentException("Invalid status");
        }

        inquiry.Status = newStatus;
        inquiry.OwnerReply = updateDto.OwnerReply;
        inquiry.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload with related data
        var updatedInquiry = await _context.RentalInquiries
            .Include(i => i.Property)
            .Include(i => i.CreatedBy)
            .Include(i => i.RentalRecord)
            .FirstAsync(i => i.Id == inquiry.Id);

        return _mapper.Map<RentalInquiryDto>(updatedInquiry);
    }

    public async Task<bool> DeleteInquiryAsync(Guid id, Guid userId, bool isAdmin = false)
    {
        var query = _context.RentalInquiries
            .Where(i => i.Id == id);

        // If not admin, only allow deleting own inquiries
        if (!isAdmin)
        {
            query = query.Where(i => i.CreatedById == userId);
        }

        var inquiry = await query.FirstOrDefaultAsync();

        if (inquiry == null)
            return false;

        _context.RentalInquiries.Remove(inquiry);
        await _context.SaveChangesAsync();

        return true;
    }
}
