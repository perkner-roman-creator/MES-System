using MiniMES.Api.Models;

namespace MiniMES.Api.DTOs;

// Work Order DTOs
// Machine DTOs
public class CreateMachineDto
{
    public required string MachineCode { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
}

public class UpdateMachineDto
{
    public string? Name { get; set; }
    public string? Type { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public double? EfficiencyRate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

public class MachineDto
{
    public int Id { get; set; }
    public required string MachineCode { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public string? Location { get; set; }
    public string Status { get; set; }
    public double EfficiencyRate { get; set; }
    public DateTime? LastMaintenanceDate { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}

// Work Order DTOs
public class CreateWorkOrderDto
{
    public required string OrderNumber { get; set; }
    public required string ProductName { get; set; }
    public int QuantityPlanned { get; set; }
    public WorkOrderPriority Priority { get; set; } = WorkOrderPriority.Normal;
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
    public int? AssignedMachineId { get; set; }
    public int? AssignedEmployeeId { get; set; }
}

public class UpdateWorkOrderDto
{
    public string? OrderNumber { get; set; }
    public string? ProductName { get; set; }
    public int? QuantityPlanned { get; set; }
    public WorkOrderPriority? Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public int? AssignedMachineId { get; set; }
    public int? AssignedEmployeeId { get; set; }
}

public class WorkOrderDto
{
    public int Id { get; set; }
    public required string OrderNumber { get; set; }
    public required string ProductName { get; set; }
    public int QuantityPlanned { get; set; }
    public int QuantityProduced { get; set; }
    public int QuantityRejected { get; set; }
    public string Status { get; set; }
    public string Priority { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Notes { get; set; }
    public double CompletionPercentage { get; set; }
}

// Employee DTOs
public class CreateEmployeeDto
{
    public string? EmployeeCode { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Position { get; set; }
    public required string Department { get; set; }
    public DateTime? HiredDate { get; set; }
    public string? Skills { get; set; }
}

public class UpdateEmployeeDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public string? Skills { get; set; }
}

public class EmployeeDto
{
    public int Id { get; set; }
    public required string EmployeeCode { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
    public required string Position { get; set; }
    public required string Department { get; set; }
    public DateTime HiredDate { get; set; }
    public string? Skills { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Production Log DTOs
public class CreateProductionLogDto
{
    public int WorkOrderId { get; set; }
    public int? MachineId { get; set; }
    public int? EmployeeId { get; set; }
    public ProductionEventType EventType { get; set; }
    public int QuantityProduced { get; set; }
    public int QuantityRejected { get; set; }
    public string? Notes { get; set; }
    public string? ReasonCode { get; set; }
}

// Auth DTOs
public class LoginDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public string? Role { get; set; }
}

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
    public required string FullName { get; set; }
}

public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

// Dashboard DTOs
public class DashboardStatsDto
{
    public int TotalActiveOrders { get; set; }
    public int TotalPendingOrders { get; set; }
    public int TotalInProgressOrders { get; set; }
    public int TotalCompletedToday { get; set; }
    public int TotalMachines { get; set; }
    public int MachinesRunning { get; set; }
    public int MachinesIdle { get; set; }
    public double MachineUtilizationRate { get; set; }
    public int TotalEmployees { get; set; }
    public int EmployeesWorking { get; set; }
    public int EmployeesAvailable { get; set; }
    public int TotalQuantityPlanned { get; set; }
    public int TotalQuantityProduced { get; set; }
    public int TotalQuantityRejected { get; set; }
    public double OverallCompletionRate { get; set; }
    public int ProductionEventsToday { get; set; }
}
