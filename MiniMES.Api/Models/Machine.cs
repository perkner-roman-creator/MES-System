namespace MiniMES.Api.Models;

public class Machine
{
    public int Id { get; set; }
    public required string MachineCode { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public MachineStatus Status { get; set; } = MachineStatus.Idle;
    public string? Location { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public double EfficiencyRate { get; set; } = 100.0;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<ProductionLog> ProductionLogs { get; set; } = new List<ProductionLog>();
}

public enum MachineStatus
{
    Idle = 0,
    Running = 1,
    Maintenance = 2,
    Error = 3,
    Offline = 4
}
