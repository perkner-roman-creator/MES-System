namespace MiniMES.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public required string EmployeeCode { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Position { get; set; }
    public required string Department { get; set; }
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Available;
    public DateTime HiredDate { get; set; }
    public string? Skills { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<WorkOrder> WorkOrders { get; set; } = new List<WorkOrder>();
    public ICollection<ProductionLog> ProductionLogs { get; set; } = new List<ProductionLog>();
    
    // Computed property
    public string FullName => $"{FirstName} {LastName}";
}

public enum EmployeeStatus
{
    Available = 0,
    Working = 1,
    OnBreak = 2,
    Absent = 3
}
