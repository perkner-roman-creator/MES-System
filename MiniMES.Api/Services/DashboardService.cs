using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;
using MiniMES.Api.Models;

namespace MiniMES.Api.Services;

public class DashboardService : IDashboardService
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProductionLogRepository _productionLogRepository;

    public DashboardService(
        IWorkOrderRepository workOrderRepository,
        IMachineRepository machineRepository,
        IEmployeeRepository employeeRepository,
        IProductionLogRepository productionLogRepository)
    {
        _workOrderRepository = workOrderRepository;
        _machineRepository = machineRepository;
        _employeeRepository = employeeRepository;
        _productionLogRepository = productionLogRepository;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var allWorkOrders = await _workOrderRepository.GetAllAsync();
        var machines = await _machineRepository.GetAllAsync();
        var employees = await _employeeRepository.GetAllAsync();
        
        var today = DateTime.UtcNow.Date;
        var todayEnd = today.AddDays(1);
        var todayLogs = await _productionLogRepository.GetByDateRangeAsync(today, todayEnd);

        var activeOrders = allWorkOrders.Where(wo => 
            wo.Status == WorkOrderStatus.InProgress || 
            wo.Status == WorkOrderStatus.Pending).ToList();

        var completedToday = allWorkOrders.Count(wo => 
            wo.Status == WorkOrderStatus.Completed && 
            wo.CompletedAt.HasValue &&
            wo.CompletedAt.Value.Date == today);

        var totalPlanned = activeOrders.Sum(wo => wo.QuantityPlanned);
        var totalProduced = activeOrders.Sum(wo => wo.QuantityProduced);
        var completionRate = totalPlanned > 0 
            ? (double)totalProduced / totalPlanned * 100 
            : 0;

        var machinesRunning = machines.Count(m => m.Status == MachineStatus.Running);
        var machineUtilization = machines.Any() 
            ? (double)machinesRunning / machines.Count() * 100 
            : 0;

        var employeesWorking = employees.Count(e => e.Status == EmployeeStatus.Working);

        return new DashboardStatsDto
        {
            TotalActiveOrders = activeOrders.Count,
            TotalPendingOrders = allWorkOrders.Count(wo => wo.Status == WorkOrderStatus.Pending),
            TotalInProgressOrders = allWorkOrders.Count(wo => wo.Status == WorkOrderStatus.InProgress),
            TotalCompletedToday = completedToday,
            TotalMachines = machines.Count(),
            MachinesRunning = machinesRunning,
            MachinesIdle = machines.Count(m => m.Status == MachineStatus.Idle),
            MachineUtilizationRate = Math.Round(machineUtilization, 1),
            TotalEmployees = employees.Count(),
            EmployeesWorking = employeesWorking,
            EmployeesAvailable = employees.Count(e => e.Status == EmployeeStatus.Available),
            TotalQuantityPlanned = totalPlanned,
            TotalQuantityProduced = totalProduced,
            TotalQuantityRejected = activeOrders.Sum(wo => wo.QuantityRejected),
            OverallCompletionRate = Math.Round(completionRate, 1),
            ProductionEventsToday = todayLogs.Count()
        };
    }
}
