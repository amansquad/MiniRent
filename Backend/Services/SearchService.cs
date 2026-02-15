using Microsoft.EntityFrameworkCore;
using MiniRent.Backend.Data;
using MiniRent.Backend.Dtos;
using MiniRent.Backend.Models;
using MiniRent.Backend.Services.Interfaces;

namespace MiniRent.Backend.Services;

public class SearchService : ISearchService
{
    private readonly AppDbContext _context;

    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<UnifiedSearchResultDto>> SearchAsync(string query, int? userId, bool isAdmin)
    {
        if (string.IsNullOrWhiteSpace(query)) return new List<UnifiedSearchResultDto>();

        var q = query.ToLower();
        var results = new List<UnifiedSearchResultDto>();

        // 1. Search Properties
        var propertiesQuery = _context.Properties.Where(p => !p.IsDeleted);
        if (!isAdmin && userId.HasValue)
        {
            // Regular user can search:
            // - Any "Available" property
            // - Any property THEY created (even if not available)
            propertiesQuery = propertiesQuery.Where(p => p.Status == PropertyStatus.Available || p.CreatedById == userId.Value);
        }

        var properties = await propertiesQuery
            .Where(p => p.Address.ToLower().Contains(q) || (p.Description != null && p.Description.ToLower().Contains(q)))
            .Take(5)
            .Select(p => new UnifiedSearchResultDto
            {
                Type = "Property",
                Id = p.Id,
                Title = p.Address,
                Subtitle = $"Rent: ${p.MonthlyRent} | {p.Bedrooms} BR",
                Url = $"/properties?id={p.Id}"
            })
            .ToListAsync();
        results.AddRange(properties);

        // 2. Search Inquiries
        var inquiriesQuery = _context.RentalInquiries.AsQueryable();
        if (!isAdmin && userId.HasValue)
        {
            inquiriesQuery = inquiriesQuery.Where(i => i.CreatedById == userId.Value || i.Property.CreatedById == userId.Value);
        }

        var inquiries = await inquiriesQuery
            .Where(i => i.Name.ToLower().Contains(q) || i.Email.ToLower().Contains(q) || (i.Message != null && i.Message.ToLower().Contains(q)))
            .Take(5)
            .Select(i => new UnifiedSearchResultDto
            {
                Type = "Inquiry",
                Id = i.Id,
                Title = i.Name,
                Subtitle = $"Email: {i.Email} | Status: {i.Status}",
                Url = $"/inquiries?id={i.Id}"
            })
            .ToListAsync();
        results.AddRange(inquiries);

        // 3. Search Rentals
        var rentalsQuery = _context.RentalRecords.AsQueryable();
        if (!isAdmin && userId.HasValue)
        {
            rentalsQuery = rentalsQuery.Where(r => r.CreatedById == userId.Value || r.Property.CreatedById == userId.Value);
        }

        var rentals = await rentalsQuery
            .Where(r => r.TenantName.ToLower().Contains(q) || r.TenantPhone.ToLower().Contains(q))
            .Take(5)
            .Select(r => new UnifiedSearchResultDto
            {
                Type = "Rental",
                Id = r.Id,
                Title = r.TenantName,
                Subtitle = $"Phone: {r.TenantPhone} | Status: {r.Status}",
                Url = $"/rentals?id={r.Id}"
            })
            .ToListAsync();
        results.AddRange(rentals);

        return results;
    }
}
