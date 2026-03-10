using MiniMES.Api.Models;
using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;

namespace MiniMES.Api.Services;

/// <summary>
/// Service implementation for managing production operations
/// </summary>
public class OperationService : IOperationService
{
    private readonly IOperationRepository _operationRepository;
    private readonly IWorkOrderRepository _workOrderRepository;

    public OperationService(IOperationRepository operationRepository, IWorkOrderRepository workOrderRepository)
    {
        _operationRepository = operationRepository;
        _workOrderRepository = workOrderRepository;
    }

    public async Task<IEnumerable<OperationDto>> GetAllOperationsAsync()
    {
        var operations = await _operationRepository.GetAllAsync();
        return operations.Select(MapToDto);
    }

    public async Task<OperationDto?> GetOperationByIdAsync(int id)
    {
        var operation = await _operationRepository.GetByIdAsync(id);
        return operation == null ? null : MapToDto(operation);
    }

    public async Task<OperationDto?> GetOperationByCodeAsync(string code)
    {
        var operation = await _operationRepository.GetByCodeAsync(code);
        return operation == null ? null : MapToDto(operation);
    }

    public async Task<IEnumerable<OperationDto>> GetOperationsByWorkOrderAsync(int workOrderId)
    {
        var operations = await _operationRepository.GetByWorkOrderAsync(workOrderId);
        return operations.Select(MapToDto);
    }

    public async Task<IEnumerable<OperationDto>> GetQualityCheckPointsAsync()
    {
        var operations = await _operationRepository.GetQualityCheckPointsAsync();
        return operations.Select(MapToDto);
    }

    public async Task<OperationDto> CreateOperationAsync(CreateOperationDto dto)
    {
        var operation = new Operation
        {
            OperationCode = dto.OperationCode,
            Name = dto.Name,
            Description = dto.Description,
            Sequence = dto.Sequence,
            EstimatedTimeMinutes = dto.EstimatedTimeMinutes,
            IsQualityCheckPoint = dto.IsQualityCheckPoint,
            RequiredMachineId = dto.RequiredMachineId,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _operationRepository.CreateAsync(operation);
        return MapToDto(created);
    }

    public async Task<OperationDto> UpdateOperationAsync(int id, UpdateOperationDto dto)
    {
        var operation = await _operationRepository.GetByIdAsync(id);
        if (operation == null)
            throw new InvalidOperationException($"Operation with ID {id} not found");

        if (dto.Name != null) operation.Name = dto.Name;
        if (dto.Description != null) operation.Description = dto.Description;
        if (dto.Sequence != null) operation.Sequence = dto.Sequence.Value;
        if (dto.EstimatedTimeMinutes != null) operation.EstimatedTimeMinutes = dto.EstimatedTimeMinutes.Value;
        if (dto.IsQualityCheckPoint != null) operation.IsQualityCheckPoint = dto.IsQualityCheckPoint.Value;
        if (dto.RequiredMachineId != null) operation.RequiredMachineId = dto.RequiredMachineId;

        await _operationRepository.UpdateAsync(operation);
        return MapToDto(operation);
    }

    public async Task DeleteOperationAsync(int id)
    {
        var operation = await _operationRepository.GetByIdAsync(id);
        if (operation == null)
            throw new InvalidOperationException($"Operation with ID {id} not found");

        operation.IsActive = false;
        await _operationRepository.UpdateAsync(operation);
    }

    public async Task<WorkOrderOperationDto> AddOperationToWorkOrderAsync(int workOrderId, CreateWorkOrderOperationDto dto)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(workOrderId);
        if (workOrder == null)
            throw new InvalidOperationException($"Work Order with ID {workOrderId} not found");

        var operation = await _operationRepository.GetByIdAsync(dto.OperationId);
        if (operation == null)
            throw new InvalidOperationException($"Operation with ID {dto.OperationId} not found");

        var workOrderOperation = new WorkOrderOperation
        {
            WorkOrderId = workOrderId,
            OperationId = dto.OperationId,
            SequenceNumber = dto.SequenceNumber,
            EstimatedQuantity = dto.EstimatedQuantity,
            Status = WorkOrderOperationStatus.Pending
        };

        // This should be saved through a proper repository, but for now we'll assume it's done
        throw new NotImplementedException("Need to create IWorkOrderOperationRepository");
    }

    public async Task<WorkOrderOperationDto> UpdateWorkOrderOperationAsync(int id, UpdateWorkOrderOperationDto dto)
    {
        throw new NotImplementedException("Need to create IWorkOrderOperationRepository");
    }

    public async Task<IEnumerable<WorkOrderOperationDto>> GetWorkOrderOperationsAsync(int workOrderId)
    {
        throw new NotImplementedException("Need to create IWorkOrderOperationRepository");
    }

    public async Task<OperationLogDto> LogOperationEventAsync(CreateOperationLogDto dto)
    {
        throw new NotImplementedException("Need to create operation log repository");
    }

    private OperationDto MapToDto(Operation operation)
    {
        return new OperationDto
        {
            Id = operation.Id,
            OperationCode = operation.OperationCode,
            Name = operation.Name,
            Description = operation.Description,
            Sequence = operation.Sequence,
            EstimatedTimeMinutes = operation.EstimatedTimeMinutes,
            IsQualityCheckPoint = operation.IsQualityCheckPoint,
            RequiredMachineId = operation.RequiredMachineId
        };
    }
}
