using MiniMES.Api.Models;
using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

/// <summary>
/// Service interface for managing production batches and serial numbers
/// </summary>
public interface IBatchService
{
    Task<IEnumerable<BatchDto>> GetAllBatchesAsync();
    Task<BatchDto?> GetBatchByIdAsync(int id);
    Task<BatchDto?> GetBatchByNumberAsync(string batchNumber);
    Task<IEnumerable<BatchDto>> GetBatchesByWorkOrderAsync(int workOrderId);
    Task<IEnumerable<BatchDto>> GetPendingQcBatchesAsync();
    Task<BatchDto> CreateBatchAsync(CreateBatchDto dto);
    Task<BatchDto> UpdateBatchAsync(int id, UpdateBatchDto dto);
    Task<BatchDto> StartBatchAsync(int id, int employeeId);
    Task<BatchDto> CompleteBatchAsync(int id);
    Task<BatchDto> SendToQcAsync(int id);
    Task DeleteBatchAsync(int id);
    
    // Serial Number operations
    Task<SerialNumberDto> CreateSerialNumberAsync(CreateSerialNumberDto dto);
    Task<IEnumerable<SerialNumberDto>> GetSerialNumbersByBatchAsync(int batchId);
    Task<SerialNumberDto?> GetSerialNumberByCodeAsync(string serial);
    Task<SerialNumberDto> ApproveQcAsync(int serialId, ApproveQcDto dto);
    Task<SerialNumberDto> RejectQcAsync(int serialId, string remarks);
    Task<SerialNumberDto> MarkAsShippedAsync(int serialId);
    
    // Batch audit trail
    Task<IEnumerable<BatchLogDto>> GetBatchLogsAsync(int batchId);
    Task<BatchLogDto> LogBatchActionAsync(CreateBatchLogDto dto);
}
