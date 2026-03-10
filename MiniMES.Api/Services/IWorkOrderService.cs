using MiniMES.Api.Models;
using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

public interface IWorkOrderService
{
    Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync();
    Task<WorkOrder?> GetWorkOrderByIdAsync(int id);
    Task<WorkOrder> CreateWorkOrderAsync(CreateWorkOrderDto dto);
    Task<WorkOrder> UpdateWorkOrderAsync(int id, UpdateWorkOrderDto dto);
    Task DeleteWorkOrderAsync(int id);
    Task<WorkOrder> StartWorkOrderAsync(int id, int? machineId, int? employeeId);
    Task<WorkOrder> PauseWorkOrderAsync(int id, string? reason);
    Task<WorkOrder> CompleteWorkOrderAsync(int id);
    Task<WorkOrder> UpdateProductionProgressAsync(int id, int quantityProduced, int quantityRejected, string? notes);
    Task<IEnumerable<WorkOrder>> GetActiveWorkOrdersAsync();
}
