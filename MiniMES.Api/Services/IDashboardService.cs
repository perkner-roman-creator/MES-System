using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

public interface IDashboardService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync();
}
