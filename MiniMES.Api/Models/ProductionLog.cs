namespace MiniMES.Api.Models;

public class ProductionLog
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ProductionEventType EventType { get; set; }
    public int QuantityProduced { get; set; }
    public int QuantityRejected { get; set; }
    public string? Notes { get; set; }
    public string? ReasonCode { get; set; }
    
    // Foreign keys
    public int WorkOrderId { get; set; }
    public WorkOrder? WorkOrder { get; set; }
    
    public int? MachineId { get; set; }
    public Machine? Machine { get; set; }
    
    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }
}

public enum ProductionEventType
{
    Start = 0,
    Production = 1,
    Pause = 2,
    Resume = 3,
    Complete = 4,
    Reject = 5,
    MachineError = 6
}
