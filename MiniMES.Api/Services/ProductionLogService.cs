using MiniMES.Api.Models;
using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;

namespace MiniMES.Api.Services;

public class ProductionLogService : IProductionLogService
{
    private readonly IProductionLogRepository _productionLogRepository;

    public ProductionLogService(IProductionLogRepository productionLogRepository)
    {
        _productionLogRepository = productionLogRepository;
    }

    public async Task<IEnumerable<ProductionLog>> GetAllLogsAsync()
    {
        return await _productionLogRepository.GetAllAsync();
    }

    public async Task<ProductionLog?> GetLogByIdAsync(int id)
    {
        return await _productionLogRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<ProductionLog>> GetLogsByWorkOrderIdAsync(int workOrderId)
    {
        return await _productionLogRepository.GetByWorkOrderIdAsync(workOrderId);
    }

    public async Task<ProductionLog> CreateLogAsync(CreateProductionLogDto dto)
    {
        var log = new ProductionLog
        {
            WorkOrderId = dto.WorkOrderId,
            MachineId = dto.MachineId,
            EmployeeId = dto.EmployeeId,
            EventType = dto.EventType,
            QuantityProduced = dto.QuantityProduced,
            QuantityRejected = dto.QuantityRejected,
            Notes = dto.Notes,
            ReasonCode = dto.ReasonCode
        };

        return await _productionLogRepository.CreateAsync(log);
    }
}
