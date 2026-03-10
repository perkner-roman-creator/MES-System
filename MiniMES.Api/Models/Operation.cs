namespace MiniMES.Api.Models;

public class Operation
{
    public int Id { get; set; }
    public required string OperationCode { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int Sequence { get; set; }  // Pořadí operace v procesu
    public int EstimatedTimeMinutes { get; set; }
    public bool IsQualityCheckPoint { get; set; } = false;
    
    // Foreign keys
    public int? RequiredMachineId { get; set; }
    public Machine? RequiredMachine { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<WorkOrderOperation> WorkOrderOperations { get; set; } = new List<WorkOrderOperation>();
    public ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();
}

/// <summary>
/// Spojuje WorkOrder s Operations (many-to-many)
/// Definuje routing (posloupnost kroků) pro konkrétní zakázku
/// </summary>
public class WorkOrderOperation
{
    public int Id { get; set; }
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    
    public int OperationId { get; set; }
    public Operation? Operation { get; set; }
    
    public int SequenceNumber { get; set; }  // Pořadí v rámci zakázky
    public int EstimatedQuantity { get; set; }
    public int CompletedQuantity { get; set; } = 0;
    public int RejectedQuantity { get; set; } = 0;
    
    public WorkOrderOperationStatus Status { get; set; } = WorkOrderOperationStatus.Pending;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    // Navigation properties
    public ICollection<OperationLog> OperationLogs { get; set; } = new List<OperationLog>();
}

public enum WorkOrderOperationStatus
{
    Pending = 0,
    InProgress = 1,
    Completed = 2,
    Failed = 3,
    Skipped = 4
}

/// <summary>
/// Log jednotlivých operací (start, stop, check, apod.)
/// </summary>
public class OperationLog
{
    public int Id { get; set; }
    public int WorkOrderOperationId { get; set; }
    public WorkOrderOperation? WorkOrderOperation { get; set; }
    
    public int? MachineId { get; set; }
    public Machine? Machine { get; set; }
    
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
    
    public OperationLogType EventType { get; set; }
    public int QuantityProcessed { get; set; }
    public int QuantityRejected { get; set; }
    public string? Reason { get; set; }
    public string? Remarks { get; set; }
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public enum OperationLogType
{
    Start = 0,
    Progress = 1,
    QualityCheck = 2,
    Rework = 3,
    Complete = 4,
    Error = 5,
    Pause = 6,
    Resume = 7
}
