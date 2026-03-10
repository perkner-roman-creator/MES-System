using MiniMES.Api.Models;
using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

public interface IProductionLogService
{
    Task<IEnumerable<ProductionLog>> GetAllLogsAsync();
    Task<ProductionLog?> GetLogByIdAsync(int id);
    Task<IEnumerable<ProductionLog>> GetLogsByWorkOrderIdAsync(int workOrderId);
    Task<ProductionLog> CreateLogAsync(CreateProductionLogDto dto);
}
