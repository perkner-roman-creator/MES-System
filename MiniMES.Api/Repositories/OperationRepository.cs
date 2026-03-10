using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

/// <summary>
/// Repository implementation for managing production operations
/// </summary>
public class OperationRepository : IOperationRepository
{
    private readonly MesDbContext _context;

    public OperationRepository(MesDbContext context)
    {
        _context = context;
    }

    public async Task<Operation?> GetByIdAsync(int id)
    {
        return await _context.Operations
            .FirstOrDefaultAsync(o => o.Id == id && o.IsActive);
    }

    public async Task<IEnumerable<Operation>> GetAllAsync()
    {
        return await _context.Operations
            .Where(o => o.IsActive)
            .ToListAsync();
    }

    public async Task<Operation> CreateAsync(Operation operation)
    {
        _context.Operations.Add(operation);
        await _context.SaveChangesAsync();
        return operation;
    }

    public async Task UpdateAsync(Operation operation)
    {
        _context.Operations.Update(operation);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Operations
            .AnyAsync(o => o.Id == id && o.IsActive);
    }

    public async Task<List<Operation>> GetByWorkOrderAsync(int workOrderId)
    {
        return await _context.Operations
            .FromSqlInterpolated($@"
                SELECT DISTINCT o.* FROM Operations o
                INNER JOIN WorkOrderOperations woo ON o.Id = woo.OperationId
                WHERE woo.WorkOrderId = {workOrderId}
                ORDER BY o.Sequence
            ")
            .ToListAsync();
    }

    public async Task<Operation?> GetByCodeAsync(string operationCode)
    {
        return await _context.Operations
            .FirstOrDefaultAsync(o => o.OperationCode == operationCode && o.IsActive);
    }

    public async Task<List<Operation>> GetActiveAsync()
    {
        return await _context.Operations
            .Where(o => o.IsActive)
            .OrderBy(o => o.Sequence)
            .ToListAsync();
    }

    public async Task<List<Operation>> GetQualityCheckPointsAsync()
    {
        return await _context.Operations
            .Where(o => o.IsQualityCheckPoint && o.IsActive)
            .OrderBy(o => o.Sequence)
            .ToListAsync();
    }
}
