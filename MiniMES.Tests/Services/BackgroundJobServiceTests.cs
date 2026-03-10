using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Services;
using MiniMES.Api.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MiniMES.Tests.Services
{
    public class BackgroundJobServiceTests
    {
        private MesDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MesDbContext(options);
        }

        [Fact]
        public async Task ProcessProductionLogs_DetectsCriticalTemperature()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = new Mock<ILogger<BackgroundJobService>>().Object;
            
            var machine = new Machine
            {
                Name = "Test Machine",
                Status = "active"
            };
            context.Machines.Add(machine);
            await context.SaveChangesAsync();

            // Přidat log s kritickou teplotou
            context.ProductionLogs.Add(new ProductionLog
            {
                MachineId = machine.Id,
                Temperature = 90.0, // Kritická!
                RecordedAt = DateTime.UtcNow.AddMinutes(-5),
                IsProcessed = false
            });
            await context.SaveChangesAsync();

            var service = new BackgroundJobService(context, logger);

            // Act
            var result = await service.ProcessProductionLogsAsync();

            // Assert
            Assert.True(result);
            var alert = context.Alerts.FirstOrDefault(a => a.MachineId == machine.Id);
            Assert.NotNull(alert);
            Assert.Contains("Kritická teplota", alert.Message);
            Assert.Equal("critical", alert.Severity);
        }

        [Fact]
        public async Task GenerateMaintenanceAlerts_CreatesAlertForOverdueMaintenanceAsync()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = new Mock<ILogger<BackgroundJobService>>().Object;

            var machine = new Machine
            {
                Name = "Old Machine",
                Status = "active",
                LastMaintenance = DateTime.UtcNow.AddDays(-40) // 40 dní od údržby
            };
            context.Machines.Add(machine);
            await context.SaveChangesAsync();

            var service = new BackgroundJobService(context, logger);

            // Act
            var result = await service.GenerateMaintenanceAlertsAsync();

            // Assert
            Assert.True(result);
            var alert = context.Alerts.FirstOrDefault(a => a.AlertType == "maintenance_overdue");
            Assert.NotNull(alert);
            Assert.Equal(machine.Id, alert.MachineId);
        }

        [Fact]
        public async Task CleanupOldData_RemovesOldProductionLogs()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = new Mock<ILogger<BackgroundJobService>>().Object;

            var machine = new Machine { Name = "Test", Status = "active" };
            context.Machines.Add(machine);
            await context.SaveChangesAsync();

            // Přidat starý log (7 měsíců zpátky)
            context.ProductionLogs.Add(new ProductionLog
            {
                MachineId = machine.Id,
                Temperature = 50,
                RecordedAt = DateTime.UtcNow.AddMonths(-7)
            });

            // Přidat nedávný log
            context.ProductionLogs.Add(new ProductionLog
            {
                MachineId = machine.Id,
                Temperature = 50,
                RecordedAt = DateTime.UtcNow.AddDays(-1)
            });

            await context.SaveChangesAsync();

            var service = new BackgroundJobService(context, logger);

            // Act
            var result = await service.CleanupOldDataAsync();

            // Assert
            Assert.True(result);
            var remainingLogs = context.ProductionLogs.ToList();
            Assert.Single(remainingLogs); // Pouze nedávný log zůstane
        }

        [Fact]
        public async Task GenerateDailyReport_CalculatesStatisticsForToday()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = new Mock<ILogger<BackgroundJobService>>().Object;

            var machine = new Machine { Name = "Test", Status = "active" };
            context.Machines.Add(machine);
            await context.SaveChangesAsync();

            // Přidat dnešní data
            context.ProductionLogs.Add(new ProductionLog
            {
                MachineId = machine.Id,
                Temperature = 50,
                RecordedAt = DateTime.UtcNow
            });

            context.Alerts.Add(new Alert
            {
                MachineId = machine.Id,
                AlertType = "test",
                Message = "Test",
                Severity = "warning",
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            var service = new BackgroundJobService(context, logger);

            // Act
            var result = await service.GenerateDailyReportAsync();

            // Assert
            Assert.True(result);
            // Logger by měl obsahovat statistiky (můžete mockovat logger a ověřit)
        }
    }
}
