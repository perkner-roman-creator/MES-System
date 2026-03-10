using MiniMES.Api.Models;
using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;

namespace MiniMES.Api.Services;

/// <summary>
/// Service implementation for managing production batches and serial numbers
/// </summary>
public class BatchService : IBatchService
{
    private readonly IBatchRepository _batchRepository;
    private readonly IWorkOrderRepository _workOrderRepository;

    public BatchService(IBatchRepository batchRepository, IWorkOrderRepository workOrderRepository)
    {
        _batchRepository = batchRepository;
        _workOrderRepository = workOrderRepository;
    }

    public async Task<IEnumerable<BatchDto>> GetAllBatchesAsync()
    {
        var batches = await _batchRepository.GetAllAsync();
        return batches.Select(MapToDto);
    }

    public async Task<BatchDto?> GetBatchByIdAsync(int id)
    {
        var batch = await _batchRepository.GetByIdAsync(id);
        return batch == null ? null : MapToDto(batch);
    }

    public async Task<BatchDto?> GetBatchByNumberAsync(string batchNumber)
    {
        var batch = await _batchRepository.GetByBatchNumberAsync(batchNumber);
        return batch == null ? null : MapToDto(batch);
    }

    public async Task<IEnumerable<BatchDto>> GetBatchesByWorkOrderAsync(int workOrderId)
    {
        var batches = await _batchRepository.GetByWorkOrderAsync(workOrderId);
        return batches.Select(MapToDto);
    }

    public async Task<IEnumerable<BatchDto>> GetPendingQcBatchesAsync()
    {
        var batches = await _batchRepository.GetPendingQcAsync();
        return batches.Select(MapToDto);
    }

    public async Task<BatchDto> CreateBatchAsync(CreateBatchDto dto)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(dto.WorkOrderId);
        if (workOrder == null)
            throw new InvalidOperationException($"Work Order with ID {dto.WorkOrderId} not found");

        var batch = new ProductBatch
        {
            BatchNumber = dto.BatchNumber,
            WorkOrderId = dto.WorkOrderId,
            MachineId = dto.MachineId,
            EmployeeId = dto.EmployeeId,
            QuantityPlanned = dto.QuantityPlanned,
            Status = BatchStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _batchRepository.CreateAsync(batch);
        
        // Log batch creation
        var log = new BatchLog
        {
            BatchId = created.Id,
            Action = "Created",
            Details = $"Batch created with {dto.QuantityPlanned} units planned",
            CreatedAt = DateTime.UtcNow
        };
        await _batchRepository.AddBatchLogAsync(log);

        return MapToDto(created);
    }

    public async Task<BatchDto> UpdateBatchAsync(int id, UpdateBatchDto dto)
    {
        var batch = await _batchRepository.GetByIdAsync(id);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {id} not found");

        if (dto.QuantityProduced != null) batch.QuantityProduced = dto.QuantityProduced.Value;
        if (dto.QuantityRejected != null) batch.QuantityRejected = dto.QuantityRejected.Value;
        if (dto.Status != null && Enum.TryParse<BatchStatus>(dto.Status, out var status))
            batch.Status = status;

        await _batchRepository.UpdateAsync(batch);
        return MapToDto(batch);
    }

    public async Task<BatchDto> StartBatchAsync(int id, int employeeId)
    {
        var batch = await _batchRepository.GetByIdAsync(id);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {id} not found");

        batch.Status = BatchStatus.InProduction;
        batch.EmployeeId = employeeId;
        await _batchRepository.UpdateAsync(batch);

        var log = new BatchLog
        {
            BatchId = batch.Id,
            Action = "Started",
            Details = $"Batch production started by employee {employeeId}",
            UserId = employeeId,
            CreatedAt = DateTime.UtcNow
        };
        await _batchRepository.AddBatchLogAsync(log);

        return MapToDto(batch);
    }

    public async Task<BatchDto> CompleteBatchAsync(int id)
    {
        var batch = await _batchRepository.GetByIdAsync(id);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {id} not found");

        batch.Status = BatchStatus.Approved;
        batch.CompletedAt = DateTime.UtcNow;
        await _batchRepository.UpdateAsync(batch);

        var log = new BatchLog
        {
            BatchId = batch.Id,
            Action = "Completed",
            Details = $"Batch completed: {batch.QuantityProduced} produced, {batch.QuantityRejected} rejected",
            CreatedAt = DateTime.UtcNow
        };
        await _batchRepository.AddBatchLogAsync(log);

        return MapToDto(batch);
    }

    public async Task<BatchDto> SendToQcAsync(int id)
    {
        var batch = await _batchRepository.GetByIdAsync(id);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {id} not found");

        batch.Status = BatchStatus.QualityCheck;
        await _batchRepository.UpdateAsync(batch);

        var log = new BatchLog
        {
            BatchId = batch.Id,
            Action = "QualityCheckStarted",
            Details = "Batch sent to quality control",
            CreatedAt = DateTime.UtcNow
        };
        await _batchRepository.AddBatchLogAsync(log);

        return MapToDto(batch);
    }

    public async Task DeleteBatchAsync(int id)
    {
        var batch = await _batchRepository.GetByIdAsync(id);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {id} not found");

        batch.IsActive = false;
        await _batchRepository.UpdateAsync(batch);
    }

    public async Task<SerialNumberDto> CreateSerialNumberAsync(CreateSerialNumberDto dto)
    {
        var batch = await _batchRepository.GetByIdAsync(dto.BatchId);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {dto.BatchId} not found");

        var serial = new SerialNumber
        {
            Serial = dto.Serial,
            BatchId = dto.BatchId,
            Status = SerialStatus.InProduction,
            ComponentsUsed = dto.ComponentsUsed,
            CreatedAt = DateTime.UtcNow
        };

        // This should be saved through a proper repository
        throw new NotImplementedException("Need to create ISerialNumberRepository");
    }

    public async Task<IEnumerable<SerialNumberDto>> GetSerialNumbersByBatchAsync(int batchId)
    {
        var serials = await _batchRepository.GetSerialNumbersByBatchAsync(batchId);
        return serials.Select(MapSerialToDto);
    }

    public async Task<SerialNumberDto?> GetSerialNumberByCodeAsync(string serial)
    {
        var serialNumber = await _batchRepository.GetSerialNumberByCodeAsync(serial);
        return serialNumber == null ? null : MapSerialToDto(serialNumber);
    }

    public async Task<SerialNumberDto> ApproveQcAsync(int serialId, ApproveQcDto dto)
    {
        throw new NotImplementedException("Need to create ISerialNumberRepository");
    }

    public async Task<SerialNumberDto> RejectQcAsync(int serialId, string remarks)
    {
        throw new NotImplementedException("Need to create ISerialNumberRepository");
    }

    public async Task<SerialNumberDto> MarkAsShippedAsync(int serialId)
    {
        throw new NotImplementedException("Need to create ISerialNumberRepository");
    }

    public async Task<IEnumerable<BatchLogDto>> GetBatchLogsAsync(int batchId)
    {
        var logs = await _batchRepository.GetBatchLogsAsync(batchId);
        return logs.Select(MapBatchLogToDto);
    }

    public async Task<BatchLogDto> LogBatchActionAsync(CreateBatchLogDto dto)
    {
        var batch = await _batchRepository.GetByIdAsync(dto.BatchId);
        if (batch == null)
            throw new InvalidOperationException($"Batch with ID {dto.BatchId} not found");

        var log = new BatchLog
        {
            BatchId = dto.BatchId,
            Action = dto.Action,
            Details = dto.Details,
            CreatedAt = DateTime.UtcNow
        };

        await _batchRepository.AddBatchLogAsync(log);
        return MapBatchLogToDto(log);
    }

    private BatchDto MapToDto(ProductBatch batch)
    {
        return new BatchDto
        {
            Id = batch.Id,
            BatchNumber = batch.BatchNumber,
            WorkOrderId = batch.WorkOrderId,
            MachineId = batch.MachineId,
            EmployeeId = batch.EmployeeId,
            QuantityPlanned = batch.QuantityPlanned,
            QuantityProduced = batch.QuantityProduced,
            QuantityRejected = batch.QuantityRejected,
            Status = batch.Status.ToString(),
            CreatedAt = batch.CreatedAt,
            CompletedAt = batch.CompletedAt,
            SerialNumbers = batch.SerialNumbers?.Select(MapSerialToDto).ToList() ?? new(),
            BatchLogs = batch.BatchLogs?.Select(MapBatchLogToDto).ToList() ?? new()
        };
    }

    private SerialNumberDto MapSerialToDto(SerialNumber serial)
    {
        return new SerialNumberDto
        {
            Id = serial.Id,
            Serial = serial.Serial,
            BatchId = serial.BatchId,
            Status = serial.Status.ToString(),
            QcResult = serial.QcResult,
            QcNotes = serial.QcNotes,
            ComponentsUsed = serial.ComponentsUsed,
            CreatedAt = serial.CreatedAt,
            QcApprovedAt = serial.QcApprovedAt
        };
    }

    private BatchLogDto MapBatchLogToDto(BatchLog log)
    {
        return new BatchLogDto
        {
            Id = log.Id,
            BatchId = log.BatchId,
            Action = log.Action,
            Details = log.Details,
            UserId = log.UserId,
            CreatedAt = log.CreatedAt
        };
    }
}
