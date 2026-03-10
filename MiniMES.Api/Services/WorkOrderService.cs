using MiniMES.Api.Models;
using MiniMES.Api.DTOs;
using MiniMES.Api.Repositories;

namespace MiniMES.Api.Services;

public class WorkOrderService : IWorkOrderService
{
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly IMachineRepository _machineRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IProductionLogRepository _productionLogRepository;

    public WorkOrderService(
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

    public async Task<IEnumerable<WorkOrder>> GetAllWorkOrdersAsync()
    {
        return await _workOrderRepository.GetAllAsync();
    }

    public async Task<WorkOrder?> GetWorkOrderByIdAsync(int id)
    {
        return await _workOrderRepository.GetByIdAsync(id);
    }

    public async Task<WorkOrder> CreateWorkOrderAsync(CreateWorkOrderDto dto)
    {
        // Check if order number already exists
        var existing = await _workOrderRepository.GetByOrderNumberAsync(dto.OrderNumber);
        if (existing != null)
        {
            throw new InvalidOperationException($"Work order with number {dto.OrderNumber} already exists.");
        }

        var workOrder = new WorkOrder
        {
            OrderNumber = dto.OrderNumber,
            ProductName = dto.ProductName,
            QuantityPlanned = dto.QuantityPlanned,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            Notes = dto.Notes,
            AssignedMachineId = dto.AssignedMachineId,
            AssignedEmployeeId = dto.AssignedEmployeeId
        };

        return await _workOrderRepository.CreateAsync(workOrder);
    }

    public async Task<WorkOrder> UpdateWorkOrderAsync(int id, UpdateWorkOrderDto dto)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            throw new KeyNotFoundException($"Work order with ID {id} not found.");
        }

        if (!string.IsNullOrWhiteSpace(dto.OrderNumber) && !string.Equals(dto.OrderNumber, workOrder.OrderNumber, StringComparison.Ordinal))
        {
            var existing = await _workOrderRepository.GetByOrderNumberAsync(dto.OrderNumber);
            if (existing != null && existing.Id != workOrder.Id)
            {
                throw new InvalidOperationException($"Work order with number {dto.OrderNumber} already exists.");
            }
            workOrder.OrderNumber = dto.OrderNumber;
        }

        workOrder.ProductName = dto.ProductName ?? workOrder.ProductName;
        workOrder.QuantityPlanned = dto.QuantityPlanned ?? workOrder.QuantityPlanned;
        workOrder.Priority = dto.Priority ?? workOrder.Priority;
        workOrder.DueDate = dto.DueDate ?? workOrder.DueDate;
        workOrder.Notes = dto.Notes ?? workOrder.Notes;
        workOrder.AssignedMachineId = dto.AssignedMachineId ?? workOrder.AssignedMachineId;
        workOrder.AssignedEmployeeId = dto.AssignedEmployeeId ?? workOrder.AssignedEmployeeId;

        await _workOrderRepository.UpdateAsync(workOrder);
        return workOrder;
    }

    public async Task DeleteWorkOrderAsync(int id)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            throw new KeyNotFoundException($"Work order with ID {id} not found.");
        }

        if (workOrder.Status == WorkOrderStatus.InProgress)
        {
            throw new InvalidOperationException("Cannot delete a work order that is in progress.");
        }

        await _workOrderRepository.DeleteAsync(id);
    }

    public async Task<WorkOrder> StartWorkOrderAsync(int id, int? machineId, int? employeeId)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            throw new KeyNotFoundException($"Work order with ID {id} not found.");
        }

        if (workOrder.Status == WorkOrderStatus.InProgress)
        {
            throw new InvalidOperationException("Work order is already in progress.");
        }

        if (workOrder.Status == WorkOrderStatus.Completed)
        {
            throw new InvalidOperationException("Cannot start a completed work order.");
        }

        workOrder.Status = WorkOrderStatus.InProgress;
        workOrder.StartedAt = DateTime.UtcNow;
        
        if (machineId.HasValue)
        {
            workOrder.AssignedMachineId = machineId.Value;
            var machine = await _machineRepository.GetByIdAsync(machineId.Value);
            if (machine != null)
            {
                machine.Status = MachineStatus.Running;
                await _machineRepository.UpdateAsync(machine);
            }
        }
        
        if (employeeId.HasValue)
        {
            workOrder.AssignedEmployeeId = employeeId.Value;
            var employee = await _employeeRepository.GetByIdAsync(employeeId.Value);
            if (employee != null)
            {
                employee.Status = EmployeeStatus.Working;
                await _employeeRepository.UpdateAsync(employee);
            }
        }

        await _workOrderRepository.UpdateAsync(workOrder);

        // Create production log
        var log = new ProductionLog
        {
            WorkOrderId = id,
            MachineId = machineId,
            EmployeeId = employeeId,
            EventType = ProductionEventType.Start,
            QuantityProduced = 0,
            QuantityRejected = 0,
            Notes = "Work order started"
        };
        await _productionLogRepository.CreateAsync(log);

        return workOrder;
    }

    public async Task<WorkOrder> PauseWorkOrderAsync(int id, string? reason)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            throw new KeyNotFoundException($"Work order with ID {id} not found.");
        }

        if (workOrder.Status != WorkOrderStatus.InProgress)
        {
            throw new InvalidOperationException("Can only pause work orders that are in progress.");
        }

        workOrder.Status = WorkOrderStatus.Paused;
        await _workOrderRepository.UpdateAsync(workOrder);

        // Create production log
        var log = new ProductionLog
        {
            WorkOrderId = id,
            MachineId = workOrder.AssignedMachineId,
            EmployeeId = workOrder.AssignedEmployeeId,
            EventType = ProductionEventType.Pause,
            QuantityProduced = 0,
            QuantityRejected = 0,
            Notes = reason ?? "Work order paused"
        };
        await _productionLogRepository.CreateAsync(log);

        return workOrder;
    }

    public async Task<WorkOrder> CompleteWorkOrderAsync(int id)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            throw new KeyNotFoundException($"Work order with ID {id} not found.");
        }

        if (workOrder.Status == WorkOrderStatus.Completed)
        {
            throw new InvalidOperationException("Work order is already completed.");
        }

        workOrder.Status = WorkOrderStatus.Completed;
        workOrder.CompletedAt = DateTime.UtcNow;
        
        // Update machine status
        if (workOrder.AssignedMachineId.HasValue)
        {
            var machine = await _machineRepository.GetByIdAsync(workOrder.AssignedMachineId.Value);
            if (machine != null)
            {
                machine.Status = MachineStatus.Idle;
                await _machineRepository.UpdateAsync(machine);
            }
        }
        
        // Update employee status
        if (workOrder.AssignedEmployeeId.HasValue)
        {
            var employee = await _employeeRepository.GetByIdAsync(workOrder.AssignedEmployeeId.Value);
            if (employee != null)
            {
                employee.Status = EmployeeStatus.Available;
                await _employeeRepository.UpdateAsync(employee);
            }
        }

        await _workOrderRepository.UpdateAsync(workOrder);

        // Create production log
        var log = new ProductionLog
        {
            WorkOrderId = id,
            MachineId = workOrder.AssignedMachineId,
            EmployeeId = workOrder.AssignedEmployeeId,
            EventType = ProductionEventType.Complete,
            QuantityProduced = 0,
            QuantityRejected = 0,
            Notes = "Work order completed"
        };
        await _productionLogRepository.CreateAsync(log);

        return workOrder;
    }

    public async Task<WorkOrder> UpdateProductionProgressAsync(int id, int quantityProduced, int quantityRejected, string? notes)
    {
        var workOrder = await _workOrderRepository.GetByIdAsync(id);
        if (workOrder == null)
        {
            throw new KeyNotFoundException($"Work order with ID {id} not found.");
        }

        if (workOrder.Status != WorkOrderStatus.InProgress)
        {
            throw new InvalidOperationException("Can only update progress for work orders that are in progress.");
        }

        workOrder.QuantityProduced += quantityProduced;
        workOrder.QuantityRejected += quantityRejected;

        await _workOrderRepository.UpdateAsync(workOrder);

        // Create production log
        var log = new ProductionLog
        {
            WorkOrderId = id,
            MachineId = workOrder.AssignedMachineId,
            EmployeeId = workOrder.AssignedEmployeeId,
            EventType = quantityRejected > 0 ? ProductionEventType.Reject : ProductionEventType.Production,
            QuantityProduced = quantityProduced,
            QuantityRejected = quantityRejected,
            Notes = notes
        };
        await _productionLogRepository.CreateAsync(log);

        return workOrder;
    }

    public async Task<IEnumerable<WorkOrder>> GetActiveWorkOrdersAsync()
    {
        return await _workOrderRepository.GetActiveOrdersAsync();
    }
}
