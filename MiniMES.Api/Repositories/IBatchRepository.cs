using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

/// <summary>
/// Repository interface for managing product batches and serial numbers
/// </summary>
public interface IBatchRepository
{
    Task<ProductBatch?> GetByIdAsync(int id);
    Task<IEnumerable<ProductBatch>> GetAllAsync();
    Task<ProductBatch> CreateAsync(ProductBatch batch);
    Task UpdateAsync(ProductBatch batch);
    Task<bool> ExistsAsync(int id);
    
    /// <summary>
    /// Get all batches for a specific work order
    /// </summary>
    Task<List<ProductBatch>> GetByWorkOrderAsync(int workOrderId);
    
    /// <summary>
    /// Get batch by batch number
    /// </summary>
    Task<ProductBatch?> GetByBatchNumberAsync(string batchNumber);
    
    /// <summary>
    /// Get all serial numbers for a batch
    /// </summary>
    Task<List<SerialNumber>> GetSerialNumbersByBatchAsync(int batchId);
    
    /// <summary>
    /// Get serial number by serial code
    /// </summary>
    Task<SerialNumber?> GetSerialNumberByCodeAsync(string serial);
    
    /// <summary>
    /// Get batches pending quality check
    /// </summary>
    Task<List<ProductBatch>> GetPendingQcAsync();
    
    /// <summary>
    /// Add batch log entry (audit trail)
    /// </summary>
    Task AddBatchLogAsync(BatchLog log);
    
    /// <summary>
    /// Get batch audit logs
    /// </summary>
    Task<List<BatchLog>> GetBatchLogsAsync(int batchId);
}
