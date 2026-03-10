using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public interface IWorkOrderRepository
{
    Task<IEnumerable<WorkOrder>> GetAllAsync();
    Task<WorkOrder?> GetByIdAsync(int id);
    Task<WorkOrder?> GetByOrderNumberAsync(string orderNumber);
    Task<IEnumerable<WorkOrder>> GetByStatusAsync(WorkOrderStatus status);
    Task<IEnumerable<WorkOrder>> GetByPriorityAsync(WorkOrderPriority priority);
    Task<IEnumerable<WorkOrder>> GetActiveOrdersAsync();
    Task<WorkOrder> CreateAsync(WorkOrder workOrder);
    Task UpdateAsync(WorkOrder workOrder);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
