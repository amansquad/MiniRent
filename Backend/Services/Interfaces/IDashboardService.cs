using MiniRent.Backend.Dtos;

namespace MiniRent.Backend.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardOverviewDto> GetDashboardOverviewAsync(Guid? userId = null, bool isAdmin = false);
    Task<SearchResultDto> GlobalSearchAsync(GlobalSearchDto searchDto, Guid? userId = null, bool isAdmin = false);
}
