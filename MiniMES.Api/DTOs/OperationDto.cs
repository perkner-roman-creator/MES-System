namespace MiniMES.Api.DTOs;

/// <summary>
/// DTO for creating a new production operation (operace)
/// </summary>
public class CreateOperationDto
{
    public required string OperationCode { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Sequence { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public bool IsQualityCheckPoint { get; set; }
    public int? RequiredMachineId { get; set; }
}

/// <summary>
/// DTO for updating a production operation
/// </summary>
public class UpdateOperationDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Sequence { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    public bool? IsQualityCheckPoint { get; set; }
    public int? RequiredMachineId { get; set; }
}

/// <summary>
/// DTO for returning operation data
/// </summary>
public class OperationDto
{
    public int Id { get; set; }
    public required string OperationCode { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Sequence { get; set; }
    public int EstimatedTimeMinutes { get; set; }
    public bool IsQualityCheckPoint { get; set; }
    public int? RequiredMachineId { get; set; }
    public MachineDto? RequiredMachine { get; set; }
}

/// <summary>
/// DTO for creating a work order operation (přiřazení operace k zakázce)
/// </summary>
public class CreateWorkOrderOperationDto
{
    public int OperationId { get; set; }
    public int SequenceNumber { get; set; }
    public int EstimatedQuantity { get; set; }
}

/// <summary>
/// DTO for updating work order operation status
/// </summary>
public class UpdateWorkOrderOperationDto
{
    public int? EstimatedQuantity { get; set; }
    public int? CompletedQuantity { get; set; }
    public int? RejectedQuantity { get; set; }
}

/// <summary>
/// DTO for returning work order operation data with routing info
/// </summary>
public class WorkOrderOperationDto
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public int OperationId { get; set; }
    public int SequenceNumber { get; set; }
    public int EstimatedQuantity { get; set; }
    public int CompletedQuantity { get; set; }
    public int RejectedQuantity { get; set; }
    public string Status { get; set; }
    public OperationDto? Operation { get; set; }
    public List<OperationLogDto> OperationLogs { get; set; } = new();
}

/// <summary>
/// DTO for logging operation events
/// </summary>
public class CreateOperationLogDto
{
    public int WorkOrderOperationId { get; set; }
    public required string EventType { get; set; } // Start, Progress, QualityCheck, Rework, Complete, Error, Pause, Resume
    public int? QuantityProcessed { get; set; }
    public int? QuantityRejected { get; set; }
    public string? Remarks { get; set; }
}

/// <summary>
/// DTO for returning operation log data
/// </summary>
public class OperationLogDto
{
    public int Id { get; set; }
    public int WorkOrderOperationId { get; set; }
    public string EventType { get; set; }
    public int? QuantityProcessed { get; set; }
    public int? QuantityRejected { get; set; }
    public string? Remarks { get; set; }
    public DateTime CreatedAt { get; set; }
}
