using MiniMES.Api.Models;

namespace MiniMES.Api.Data;

public static class DbInitializer
{
    public static void Initialize(MesDbContext context)
    {
        // Reset demo data so each start has deterministic, current dataset.
        context.BatchLogs.RemoveRange(context.BatchLogs);
        context.SerialNumbers.RemoveRange(context.SerialNumbers);
        context.ProductBatches.RemoveRange(context.ProductBatches);
        context.OperationLogs.RemoveRange(context.OperationLogs);
        context.WorkOrderOperations.RemoveRange(context.WorkOrderOperations);
        context.ProductionLogs.RemoveRange(context.ProductionLogs);
        context.WorkOrders.RemoveRange(context.WorkOrders);
        context.Operations.RemoveRange(context.Operations);
        context.Machines.RemoveRange(context.Machines);
        context.Employees.RemoveRange(context.Employees);
        context.Users.RemoveRange(context.Users);
        context.SaveChanges();

        var today = DateTime.UtcNow.Date;

        var users = new User[]
        {
            new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Email = "admin@email.cz",
                FullName = "System Administrator",
                Role = "Admin"
            },
            new User
            {
                Username = "manager",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                Email = "manager@email.cz",
                FullName = "Production Manager",
                Role = "Manager"
            },
            new User
            {
                Username = "operator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("operator123"),
                Email = "operator@email.cz",
                FullName = "Machine Operator",
                Role = "Operator"
            }
        };
        context.Users.AddRange(users);
        context.SaveChanges();

        var machines = new Machine[]
        {
            new Machine
            {
                MachineCode = "CNC-001",
                Name = "CNC fréza 1",
                Type = "CNC fréza",
                Location = "Výrobní hala A",
                Status = MachineStatus.Running,
                EfficiencyRate = 91.9,
                LastMaintenanceDate = today.AddDays(-21),
                NextMaintenanceDate = today.AddDays(14)
            },
            new Machine
            {
                MachineCode = "CNC-002",
                Name = "CNC soustruh 1",
                Type = "CNC soustruh",
                Location = "Výrobní hala A",
                Status = MachineStatus.Idle,
                EfficiencyRate = 88.5,
                LastMaintenanceDate = today.AddDays(-10),
                NextMaintenanceDate = today.AddDays(20)
            },
            new Machine
            {
                MachineCode = "WELD-001",
                Name = "Robotický svářeč 1",
                Type = "Robotický svářeč",
                Location = "Výrobní hala B",
                Status = MachineStatus.Running,
                EfficiencyRate = 97.6,
                LastMaintenanceDate = today.AddDays(-28),
                NextMaintenanceDate = today.AddDays(7)
            },
            new Machine
            {
                MachineCode = "ASSY-001",
                Name = "Montážní linka 1",
                Type = "Montáž",
                Location = "Výrobní hala C",
                Status = MachineStatus.Idle,
                EfficiencyRate = 89.7,
                LastMaintenanceDate = today.AddDays(-15),
                NextMaintenanceDate = today.AddDays(15)
            }
        };
        context.Machines.AddRange(machines);
        context.SaveChanges();

        var employees = new Employee[]
        {
            new Employee
            {
                EmployeeCode = "EMP-001",
                FirstName = "Adam",
                LastName = "Novák",
                Email = "adam.novak@email.cz",
                Phone = "+420 123 456 789",
                Position = "Operátor CNC",
                Department = "Výroba",
                Status = EmployeeStatus.Working,
                HiredDate = new DateTime(2021, 2, 1),
                Skills = "Programování CNC, kontrola kvality"
            },
            new Employee
            {
                EmployeeCode = "EMP-002",
                FirstName = "Petra",
                LastName = "Svobodová",
                Email = "petra.svobodova@email.cz",
                Phone = "+420 123 456 790",
                Position = "Svářeč",
                Department = "Výroba",
                Status = EmployeeStatus.Available,
                HiredDate = new DateTime(2020, 6, 1),
                Skills = "Svařování MIG, svařování TIG"
            },
            new Employee
            {
                EmployeeCode = "EMP-003",
                FirstName = "Martin",
                LastName = "Dvořák",
                Email = "martin.dvorak@email.cz",
                Phone = "+420 123 456 791",
                Position = "Technik montáže",
                Department = "Výroba",
                Status = EmployeeStatus.OnBreak,
                HiredDate = new DateTime(2022, 3, 10),
                Skills = "Montáž, testování, balení"
            },
            new Employee
            {
                EmployeeCode = "EMP-004",
                FirstName = "Eva",
                LastName = "Černá",
                Email = "eva.cerna@email.cz",
                Phone = "+420 123 456 792",
                Position = "Kontrolor kvality",
                Department = "Kvalita",
                Status = EmployeeStatus.Absent,
                HiredDate = new DateTime(2019, 9, 5),
                Skills = "Kontrola kvality, dokumentace"
            }
        };
        context.Employees.AddRange(employees);
        context.SaveChanges();

        // Exactly 4 active work orders for dashboard and work orders list.
        var workOrders = new WorkOrder[]
        {
            new WorkOrder
            {
                OrderNumber = "WO-2026-001",
                ProductName = "Přesná převodová sestava",
                QuantityPlanned = 120,
                QuantityProduced = 52,
                QuantityRejected = 2,
                Status = WorkOrderStatus.InProgress,
                Priority = WorkOrderPriority.High,
                DueDate = today.AddDays(1),
                AssignedMachineId = machines[0].Id,
                AssignedEmployeeId = employees[0].Id,
                StartedAt = today.AddDays(-1).AddHours(8),
                Notes = "Priority zákazník - expedice zítra"
            },
            new WorkOrder
            {
                OrderNumber = "WO-2026-002",
                ProductName = "Ocelový držák typ A",
                QuantityPlanned = 200,
                QuantityProduced = 0,
                QuantityRejected = 0,
                Status = WorkOrderStatus.Pending,
                Priority = WorkOrderPriority.Normal,
                DueDate = today.AddDays(2),
                AssignedMachineId = machines[3].Id,
                AssignedEmployeeId = employees[3].Id,
                Notes = "Čeká na materiál"
            },
            new WorkOrder
            {
                OrderNumber = "WO-2026-003",
                ProductName = "Železný rám",
                QuantityPlanned = 150,
                QuantityProduced = 15,
                QuantityRejected = 1,
                Status = WorkOrderStatus.Pending,
                Priority = WorkOrderPriority.Low,
                DueDate = today.AddDays(4),
                AssignedMachineId = machines[2].Id,
                AssignedEmployeeId = employees[1].Id,
                Notes = "Příprava výrobní série"
            },
            new WorkOrder
            {
                OrderNumber = "WO-2026-004",
                ProductName = "Montáž řídicího panelu",
                QuantityPlanned = 80,
                QuantityProduced = 24,
                QuantityRejected = 0,
                Status = WorkOrderStatus.InProgress,
                Priority = WorkOrderPriority.Urgent,
                DueDate = today,
                AssignedMachineId = machines[1].Id,
                AssignedEmployeeId = employees[2].Id,
                StartedAt = today.AddHours(6),
                Notes = "Urgentní servisní zakázka"
            }
        };
        context.WorkOrders.AddRange(workOrders);
        context.SaveChanges();

        var productionLogs = new ProductionLog[]
        {
            new ProductionLog
            {
                WorkOrderId = workOrders[0].Id,
                MachineId = machines[0].Id,
                EmployeeId = employees[0].Id,
                EventType = ProductionEventType.Start,
                QuantityProduced = 0,
                QuantityRejected = 0,
                Timestamp = today.AddDays(-1).AddHours(8),
                Notes = "Zakázka spuštěna"
            },
            new ProductionLog
            {
                WorkOrderId = workOrders[0].Id,
                MachineId = machines[0].Id,
                EmployeeId = employees[0].Id,
                EventType = ProductionEventType.Production,
                QuantityProduced = 32,
                QuantityRejected = 1,
                Timestamp = today.AddDays(-1).AddHours(14),
                Notes = "První výrobní dávka"
            },
            new ProductionLog
            {
                WorkOrderId = workOrders[0].Id,
                MachineId = machines[0].Id,
                EmployeeId = employees[0].Id,
                EventType = ProductionEventType.Production,
                QuantityProduced = 20,
                QuantityRejected = 1,
                Timestamp = today.AddHours(10),
                Notes = "Druhá výrobní dávka"
            },
            new ProductionLog
            {
                WorkOrderId = workOrders[3].Id,
                MachineId = machines[1].Id,
                EmployeeId = employees[2].Id,
                EventType = ProductionEventType.Start,
                QuantityProduced = 0,
                QuantityRejected = 0,
                Timestamp = today.AddHours(6),
                Notes = "Montáž spuštěna"
            },
            new ProductionLog
            {
                WorkOrderId = workOrders[3].Id,
                MachineId = machines[1].Id,
                EmployeeId = employees[2].Id,
                EventType = ProductionEventType.Production,
                QuantityProduced = 24,
                QuantityRejected = 0,
                Timestamp = today.AddHours(11),
                Notes = "Probíhá montáž panelů"
            }
        };
        context.ProductionLogs.AddRange(productionLogs);
        context.SaveChanges();
    }
}
