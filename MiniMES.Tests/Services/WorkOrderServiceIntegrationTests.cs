using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Services;
using MiniMES.Api.Repositories;
using MiniMES.Api.Models;
using System.Threading.Tasks;

namespace MiniMES.Tests.Services
{
    public class WorkOrderServiceIntegrationTests
    {
        private MesDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MesDbContext(options);
        }

        [Fact]
        public async Task CreateWorkOrder_WithValidData_ReturnsWorkOrder()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new WorkOrderRepository(context);
            var service = new WorkOrderService(repository);

            var workOrder = new WorkOrder
            {
                OrderNumber = "WO-001",
                ProductCode = "PROD-123",
                Quantity = 100,
                DueDate = DateTime.UtcNow.AddDays(7),
                Status = "Pending"
            };

            // Act
            var result = await repository.CreateAsync(workOrder);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WO-001", result.OrderNumber);
            Assert.Equal(100, result.Quantity);
        }

        [Fact]
        public async Task GetWorkOrder_WithValidId_ReturnsWorkOrder()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new WorkOrderRepository(context);

            var workOrder = new WorkOrder
            {
                OrderNumber = "WO-002",
                ProductCode = "PROD-124",
                Quantity = 50,
                DueDate = DateTime.UtcNow.AddDays(5),
                Status = "In Progress"
            };

            context.WorkOrders.Add(workOrder);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(workOrder.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("WO-002", result.OrderNumber);
        }

        [Fact]
        public async Task UpdateWorkOrder_WithValidData_UpdatesSuccessfully()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new WorkOrderRepository(context);

            var workOrder = new WorkOrder
            {
                OrderNumber = "WO-003",
                ProductCode = "PROD-125",
                Quantity = 75,
                DueDate = DateTime.UtcNow.AddDays(3),
                Status = "Pending"
            };

            context.WorkOrders.Add(workOrder);
            await context.SaveChangesAsync();

            // Act
            workOrder.Status = "Completed";
            workOrder.Quantity = 75;
            await repository.UpdateAsync(workOrder);
            await context.SaveChangesAsync();

            var updated = await repository.GetByIdAsync(workOrder.Id);

            // Assert
            Assert.Equal("Completed", updated.Status);
        }
    }
}
