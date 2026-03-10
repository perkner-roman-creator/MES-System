using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public interface IProductionLogRepository
{
    Task<IEnumerable<ProductionLog>> GetAllAsync();
    Task<ProductionLog?> GetByIdAsync(int id);
    Task<IEnumerable<ProductionLog>> GetByWorkOrderIdAsync(int workOrderId);
    Task<IEnumerable<ProductionLog>> GetByMachineIdAsync(int machineId);
    Task<IEnumerable<ProductionLog>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<ProductionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ProductionLog> CreateAsync(ProductionLog productionLog);
    Task UpdateAsync(ProductionLog productionLog);
    Task DeleteAsync(int id);
}
