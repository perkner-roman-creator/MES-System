namespace MiniMES.Api.Models;

public class WorkOrder
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    public required string ProductName { get; set; }
    public int QuantityPlanned { get; set; }
    public int QuantityProduced { get; set; } = 0;
    public int QuantityRejected { get; set; } = 0;
    public WorkOrderStatus Status { get; set; } = WorkOrderStatus.Pending;
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
    
    // Foreign keys
    public int? AssignedMachineId { get; set; }
    public Machine? AssignedMachine { get; set; }
    
    public int? AssignedEmployeeId { get; set; }
    public Employee? AssignedEmployee { get; set; }
    
    // Navigation properties
    public ICollection<ProductionLog> ProductionLogs { get; set; } = new List<ProductionLog>();
    public ICollection<WorkOrderOperation> WorkOrderOperations { get; set; } = new List<WorkOrderOperation>();
    public ICollection<ProductBatch> ProductBatches { get; set; } = new List<ProductBatch>();
    
    // Calculated properties
    public double CompletionPercentage => QuantityPlanned > 0 
        ? (double)QuantityProduced / QuantityPlanned * 100 
        : 0;
    
    public int QuantityRemaining => QuantityPlanned - QuantityProduced;
}

public enum WorkOrderStatus
{
    Pending = 0,
    InProgress = 1,
    Paused = 2,
    Completed = 3,
    Cancelled = 4
}

public enum WorkOrderPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
