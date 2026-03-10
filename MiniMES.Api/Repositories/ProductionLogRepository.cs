using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public class ProductionLogRepository : IProductionLogRepository
{
    private readonly MesDbContext _context;

    public ProductionLogRepository(MesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductionLog>> GetAllAsync()
    {
        return await _context.ProductionLogs
            .Include(pl => pl.WorkOrder)
            .Include(pl => pl.Machine)
            .Include(pl => pl.Employee)
            .OrderByDescending(pl => pl.Timestamp)
            .ToListAsync();
    }

    public async Task<ProductionLog?> GetByIdAsync(int id)
    {
        return await _context.ProductionLogs
            .Include(pl => pl.WorkOrder)
            .Include(pl => pl.Machine)
            .Include(pl => pl.Employee)
            .FirstOrDefaultAsync(pl => pl.Id == id);
    }

    public async Task<IEnumerable<ProductionLog>> GetByWorkOrderIdAsync(int workOrderId)
    {
        return await _context.ProductionLogs
            .Include(pl => pl.Machine)
            .Include(pl => pl.Employee)
            .Where(pl => pl.WorkOrderId == workOrderId)
            .OrderBy(pl => pl.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductionLog>> GetByMachineIdAsync(int machineId)
    {
        return await _context.ProductionLogs
            .Include(pl => pl.WorkOrder)
            .Include(pl => pl.Employee)
            .Where(pl => pl.MachineId == machineId)
            .OrderByDescending(pl => pl.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductionLog>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _context.ProductionLogs
            .Include(pl => pl.WorkOrder)
            .Include(pl => pl.Machine)
            .Where(pl => pl.EmployeeId == employeeId)
            .OrderByDescending(pl => pl.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductionLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.ProductionLogs
            .Include(pl => pl.WorkOrder)
            .Include(pl => pl.Machine)
            .Include(pl => pl.Employee)
            .Where(pl => pl.Timestamp >= startDate && pl.Timestamp <= endDate)
            .OrderBy(pl => pl.Timestamp)
            .ToListAsync();
    }

    public async Task<ProductionLog> CreateAsync(ProductionLog productionLog)
    {
        _context.ProductionLogs.Add(productionLog);
        await _context.SaveChangesAsync();
        return productionLog;
    }

    public async Task UpdateAsync(ProductionLog productionLog)
    {
        _context.Entry(productionLog).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var log = await _context.ProductionLogs.FindAsync(id);
        if (log != null)
        {
            _context.ProductionLogs.Remove(log);
            await _context.SaveChangesAsync();
        }
    }
}
