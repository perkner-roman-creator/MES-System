using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

/// <summary>
/// Repository implementation for managing product batches and serial numbers
/// </summary>
public class BatchRepository : IBatchRepository
{
    private readonly MesDbContext _context;

    public BatchRepository(MesDbContext context)
    {
        _context = context;
    }

    public async Task<ProductBatch?> GetByIdAsync(int id)
    {
        return await _context.ProductBatches
            .Include(b => b.SerialNumbers)
            .Include(b => b.BatchLogs)
            .FirstOrDefaultAsync(b => b.Id == id && b.IsActive);
    }

    public async Task<IEnumerable<ProductBatch>> GetAllAsync()
    {
        return await _context.ProductBatches
            .Where(b => b.IsActive)
            .Include(b => b.SerialNumbers)
            .ToListAsync();
    }

    public async Task<ProductBatch> CreateAsync(ProductBatch batch)
    {
        _context.ProductBatches.Add(batch);
        await _context.SaveChangesAsync();
        return batch;
    }

    public async Task UpdateAsync(ProductBatch batch)
    {
        _context.ProductBatches.Update(batch);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.ProductBatches
            .AnyAsync(b => b.Id == id && b.IsActive);
    }

    public async Task<List<ProductBatch>> GetByWorkOrderAsync(int workOrderId)
    {
        return await _context.ProductBatches
            .Where(b => b.WorkOrderId == workOrderId && b.IsActive)
            .Include(b => b.SerialNumbers)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<ProductBatch?> GetByBatchNumberAsync(string batchNumber)
    {
        return await _context.ProductBatches
            .Include(b => b.SerialNumbers)
            .Include(b => b.BatchLogs)
            .FirstOrDefaultAsync(b => b.BatchNumber == batchNumber && b.IsActive);
    }

    public async Task<List<SerialNumber>> GetSerialNumbersByBatchAsync(int batchId)
    {
        return await _context.SerialNumbers
            .Where(s => s.BatchId == batchId)
            .ToListAsync();
    }

    public async Task<SerialNumber?> GetSerialNumberByCodeAsync(string serial)
    {
        return await _context.SerialNumbers
            .FirstOrDefaultAsync(s => s.Serial == serial);
    }

    public async Task<List<ProductBatch>> GetPendingQcAsync()
    {
        return await _context.ProductBatches
            .Where(b => b.Status == BatchStatus.QualityCheck && b.IsActive)
            .Include(b => b.SerialNumbers)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task AddBatchLogAsync(BatchLog log)
    {
        _context.BatchLogs.Add(log);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BatchLog>> GetBatchLogsAsync(int batchId)
    {
        return await _context.BatchLogs
            .Where(bl => bl.BatchId == batchId)
            .OrderByDescending(bl => bl.CreatedAt)
            .ToListAsync();
    }
}
