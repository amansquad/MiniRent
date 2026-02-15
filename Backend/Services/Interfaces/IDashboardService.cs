using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetDashboardOverviewAsync(int? userId = null, bool isAdmin = false);
    Task<SearchResultDto> GlobalSearchAsync(GlobalSearchDto searchDto, int? userId = null, bool isAdmin = false);
}
