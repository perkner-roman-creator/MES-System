using MiniMES.Api.Models;
using MiniMES.Api.DTOs;

namespace MiniMES.Api.Services;

/// <summary>
/// Service interface for managing production operations
/// </summary>
public interface IOperationService
{
    Task<IEnumerable<OperationDto>> GetAllOperationsAsync();
    Task<OperationDto?> GetOperationByIdAsync(int id);
    Task<OperationDto?> GetOperationByCodeAsync(string code);
    Task<IEnumerable<OperationDto>> GetOperationsByWorkOrderAsync(int workOrderId);
    Task<IEnumerable<OperationDto>> GetQualityCheckPointsAsync();
    Task<OperationDto> CreateOperationAsync(CreateOperationDto dto);
    Task<OperationDto> UpdateOperationAsync(int id, UpdateOperationDto dto);
    Task DeleteOperationAsync(int id);
    Task<WorkOrderOperationDto> AddOperationToWorkOrderAsync(int workOrderId, CreateWorkOrderOperationDto dto);
    Task<WorkOrderOperationDto> UpdateWorkOrderOperationAsync(int id, UpdateWorkOrderOperationDto dto);
    Task<IEnumerable<WorkOrderOperationDto>> GetWorkOrderOperationsAsync(int workOrderId);
    Task<OperationLogDto> LogOperationEventAsync(CreateOperationLogDto dto);
}
