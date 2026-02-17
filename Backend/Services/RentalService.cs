using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class RentalService : IRentalService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RentalService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<(List<RentalRecordDto> Rentals, int TotalCount)> GetRentalsAsync(RentalFilterDto filter, Guid? userId = null, bool isAdmin = false)
    {
        var query = _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Include(r => r.Payments)
            .AsQueryable();

        // If not admin, filter by user's own rentals OR rentals of user's properties
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(r => r.CreatedById == userId.Value || r.Property.CreatedById == userId.Value);
        }

        // Apply filters
        if (filter.PropertyId.HasValue)
        {
            query = query.Where(r => r.PropertyId == filter.PropertyId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<RentalStatus>(filter.Status, true, out var status))
        {
            query = query.Where(r => r.Status == status);
        }

        if (filter.StartDateFrom.HasValue)
        {
            query = query.Where(r => r.StartDate >= filter.StartDateFrom.Value);
        }

        if (filter.StartDateTo.HasValue)
        {
            query = query.Where(r => r.StartDate <= filter.StartDateTo.Value);
        }

        if (!string.IsNullOrEmpty(filter.TenantName))
        {
            query = query.Where(r => r.TenantName.Contains(filter.TenantName));
        }
        var totalCount = await query.CountAsync();

        var rentals = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return (_mapper.Map<List<RentalRecordDto>>(rentals), totalCount);
    }

    public async Task<RentalRecordDto?> GetRentalByIdAsync(Guid id, Guid? userId = null, bool isAdmin = false)
    {
        var query = _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Include(r => r.Payments)
            .Where(r => r.Id == id);

        // If not admin, filter by user's own rentals OR rentals of user's properties
        if (!isAdmin && userId.HasValue)
        {
            query = query.Where(r => r.CreatedById == userId.Value || r.Property.CreatedById == userId.Value);
        }

        var rental = await query.FirstOrDefaultAsync();

        return rental != null ? _mapper.Map<RentalRecordDto>(rental) : null;
    }

    public async Task<RentalRecordDto> CreateRentalAsync(RentalCreateDto createDto, Guid userId)
    {
        // Check if property exists and is available
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == createDto.PropertyId && !p.IsDeleted);

        if (property == null)
        {
            throw new ArgumentException("Property not found");
        }

        // Create rental record
        var rental = _mapper.Map<RentalRecord>(createDto);
        rental.CreatedAt = DateTime.UtcNow;
        rental.CreatedById = userId;

        // If owner creates it, it's Active immediately.
        // If tenant requests it, it's Pending.
        var isOwner = property.CreatedById == userId;
        rental.Status = isOwner ? RentalStatus.Active : RentalStatus.Pending;

        _context.RentalRecords.Add(rental);

        if (createDto.InquiryId.HasValue)
        {
            var inquiry = await _context.RentalInquiries
                .FirstOrDefaultAsync(i => i.Id == createDto.InquiryId.Value);
            
            if (inquiry != null)
            {
                inquiry.Status = InquiryStatus.Converted;
                inquiry.RentalRecordId = rental.Id;
            }
        }

        if (isOwner)
        {
            // Update property status to Rented
            property.Status = PropertyStatus.Rented;
            property.UpdatedAt = DateTime.UtcNow;
            property.UpdatedById = userId;
        }

        await _context.SaveChangesAsync();

        // Reload with related data
        var createdRental = await _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Include(r => r.Payments)
            .Include(r => r.Tenant)
            .FirstAsync(r => r.Id == rental.Id);

        return _mapper.Map<RentalRecordDto>(createdRental);
    }

    public async Task<RentalRecordDto?> UpdateRentalAsync(RentalUpdateDto updateDto, Guid userId, bool isAdmin = false)
    {
        var query = _context.RentalRecords
            .Include(r => r.Property)
            .Where(r => r.Id == updateDto.Id);

        // If not admin, only allow updating own rentals
        if (!isAdmin)
        {
            query = query.Where(r => r.CreatedById == userId);
        }

        var rental = await query.FirstOrDefaultAsync();

        if (rental == null)
            return null;

        _mapper.Map(updateDto, rental);
        rental.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Reload with related data
        var updatedRental = await _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Include(r => r.Payments)
            .Include(r => r.Tenant)
            .FirstAsync(r => r.Id == rental.Id);

        return _mapper.Map<RentalRecordDto>(updatedRental);
    }

    public async Task<RentalRecordDto?> UpdateStatusAsync(Guid id, string status, Guid userId, bool isAdmin = false)
    {
        var query = _context.RentalRecords
            .Include(r => r.Property)
            .Where(r => r.Id == id);

        // If not admin, only property owner can update status (Approve/Reject)
        if (!isAdmin)
        {
            query = query.Where(r => r.Property.CreatedById == userId);
        }

        var rental = await query.FirstOrDefaultAsync();
        if (rental == null) return null;

        if (Enum.TryParse<RentalStatus>(status, true, out var newStatus))
        {
            // If approves (Pending -> Active), update property status
            if (rental.Status == RentalStatus.Pending && newStatus == RentalStatus.Active)
            {
                rental.Property.Status = PropertyStatus.Rented;
                rental.Property.UpdatedAt = DateTime.UtcNow;
                rental.Property.UpdatedById = userId;
            }
            // If rental ends or is terminated (Active -> Ended/Terminated), free the property
            else if ((rental.Status == RentalStatus.Active || rental.Status == RentalStatus.Pending) && 
                     (newStatus == RentalStatus.Ended || newStatus == RentalStatus.Terminated || newStatus == RentalStatus.Rejected))
            {
                rental.Property.Status = PropertyStatus.Available;
                rental.Property.UpdatedAt = DateTime.UtcNow;
                rental.Property.UpdatedById = userId;

                // Should we set EndDate if not set? 
                if (newStatus == RentalStatus.Ended || newStatus == RentalStatus.Terminated)
                {
                    if (!rental.EndDate.HasValue)
                    {
                        rental.EndDate = DateTime.UtcNow;
                    }
                }
            }
            
            rental.Status = newStatus;
            rental.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        // Reload with related data
        var updatedRental = await _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Include(r => r.Payments)
            .Include(r => r.Tenant)
            .FirstAsync(r => r.Id == rental.Id);

        return _mapper.Map<RentalRecordDto>(updatedRental);
    }

    public async Task<RentalRecordDto?> EndRentalAsync(Guid id, RentalEndDto endDto, Guid userId, bool isAdmin = false)
    {
        var query = _context.RentalRecords
            .Include(r => r.Property)
            .Where(r => r.Id == id);

        // If not admin, only allow ending own rentals
        if (!isAdmin)
        {
            query = query.Where(r => r.CreatedById == userId);
        }

        var rental = await query.FirstOrDefaultAsync();

        if (rental == null || rental.Status != RentalStatus.Active)
            return null;

        rental.EndDate = endDto.EndDate;
        rental.Status = RentalStatus.Ended;
        rental.UpdatedAt = DateTime.UtcNow;

        // Update property status to Available
        rental.Property.Status = PropertyStatus.Available;
        rental.Property.UpdatedAt = DateTime.UtcNow;
        rental.Property.UpdatedById = userId;

        if (!string.IsNullOrEmpty(endDto.Notes))
        {
            rental.Notes = string.IsNullOrEmpty(rental.Notes)
                ? endDto.Notes
                : $"{rental.Notes}\n\nEnded: {endDto.Notes}";
        }

        await _context.SaveChangesAsync();

        // Reload with related data
        var updatedRental = await _context.RentalRecords
            .Include(r => r.Property)
            .Include(r => r.CreatedBy)
            .Include(r => r.Payments)
            .Include(r => r.Tenant)
            .FirstAsync(r => r.Id == rental.Id);

        return _mapper.Map<RentalRecordDto>(updatedRental);
    }

    public async Task<bool> DeleteRentalAsync(Guid id, Guid userId, bool isAdmin = false)
    {
        var query = _context.RentalRecords
            .Include(r => r.Property)
            .Where(r => r.Id == id);

        var rental = await query.FirstOrDefaultAsync();

        if (rental == null)
            return false;

        // Permission check
        if (!isAdmin)
        {
            // Only creator can delete their own
            if (rental.CreatedById != userId)
            {
                return false;
            }

            // Creator cannot delete Active rentals (must end them first)
            if (rental.Status == RentalStatus.Active)
            {
                return false;
            }
        }

        _context.RentalRecords.Remove(rental);
        await _context.SaveChangesAsync();
        return true;
    }
}
