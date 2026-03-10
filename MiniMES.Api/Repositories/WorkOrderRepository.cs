using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

public class WorkOrderRepository : IWorkOrderRepository
{
    private readonly MesDbContext _context;

    public WorkOrderRepository(MesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WorkOrder>> GetAllAsync()
    {
        return await _context.WorkOrders
            .Include(wo => wo.AssignedMachine)
            .Include(wo => wo.AssignedEmployee)
            .Include(wo => wo.ProductionLogs)
            .OrderByDescending(wo => wo.Priority)
            .ThenBy(wo => wo.DueDate)
            .ToListAsync();
    }

    public async Task<WorkOrder?> GetByIdAsync(int id)
    {
        return await _context.WorkOrders
            .Include(wo => wo.AssignedMachine)
            .Include(wo => wo.AssignedEmployee)
            .Include(wo => wo.ProductionLogs)
            .FirstOrDefaultAsync(wo => wo.Id == id);
    }

    public async Task<WorkOrder?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _context.WorkOrders
            .Include(wo => wo.AssignedMachine)
            .Include(wo => wo.AssignedEmployee)
            .FirstOrDefaultAsync(wo => wo.OrderNumber == orderNumber);
    }

    public async Task<IEnumerable<WorkOrder>> GetByStatusAsync(WorkOrderStatus status)
    {
        return await _context.WorkOrders
            .Include(wo => wo.AssignedMachine)
            .Include(wo => wo.AssignedEmployee)
            .Where(wo => wo.Status == status)
            .OrderBy(wo => wo.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkOrder>> GetByPriorityAsync(WorkOrderPriority priority)
    {
        return await _context.WorkOrders
            .Include(wo => wo.AssignedMachine)
            .Include(wo => wo.AssignedEmployee)
            .Where(wo => wo.Priority == priority)
            .OrderBy(wo => wo.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkOrder>> GetActiveOrdersAsync()
    {
        return await _context.WorkOrders
            .Include(wo => wo.AssignedMachine)
            .Include(wo => wo.AssignedEmployee)
            .Where(wo => wo.Status == WorkOrderStatus.InProgress || wo.Status == WorkOrderStatus.Pending)
            .OrderByDescending(wo => wo.Priority)
            .ThenBy(wo => wo.DueDate)
            .ToListAsync();
    }

    public async Task<WorkOrder> CreateAsync(WorkOrder workOrder)
    {
        _context.WorkOrders.Add(workOrder);
        await _context.SaveChangesAsync();
        return workOrder;
    }

    public async Task UpdateAsync(WorkOrder workOrder)
    {
        _context.Entry(workOrder).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var workOrder = await _context.WorkOrders.FindAsync(id);
        if (workOrder != null)
        {
            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.WorkOrders.AnyAsync(wo => wo.Id == id);
    }
}
