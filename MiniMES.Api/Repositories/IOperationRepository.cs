using MiniMES.Api.Models;

namespace MiniMES.Api.Repositories;

/// <summary>
/// Repository interface for managing production operations
/// </summary>
public interface IOperationRepository
{
    Task<Operation?> GetByIdAsync(int id);
    Task<IEnumerable<Operation>> GetAllAsync();
    Task<Operation> CreateAsync(Operation operation);
    Task UpdateAsync(Operation operation);
    Task<bool> ExistsAsync(int id);
    
    /// <summary>
    /// Get all operations for a specific work order
    /// </summary>
    Task<List<Operation>> GetByWorkOrderAsync(int workOrderId);
    
    /// <summary>
    /// Get operation by operation code
    /// </summary>
    Task<Operation?> GetByCodeAsync(string operationCode);
    
    /// <summary>
    /// Get all active operations
    /// </summary>
    Task<List<Operation>> GetActiveAsync();
    
    /// <summary>
    /// Get operations that are quality check points
    /// </summary>
    Task<List<Operation>> GetQualityCheckPointsAsync();
}
