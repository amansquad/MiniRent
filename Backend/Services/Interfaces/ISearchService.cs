using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface ISearchService
{
    Task<List<UnifiedSearchResultDto>> SearchAsync(string query, int? userId, bool isAdmin);
}
