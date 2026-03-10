using Xunit;
using Microsoft.EntityFrameworkCore;
using MiniMES.Api.Data;
using MiniMES.Api.Services;
using MiniMES.Api.Repositories;
using MiniMES.Api.Models;
using System.Threading.Tasks;

namespace MiniMES.Tests.Services
{
    public class MachineServiceIntegrationTests
    {
        private MesDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MesDbContext(options);
        }

        [Fact]
        public async Task CreateMachine_WithValidData_ReturnsMachine()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new MachineRepository(context);

            var machine = new Machine
            {
                Code = "MACH-001",
                Name = "CNC Lathe",
                Type = "Lathe",
                Location = "Floor 1",
                Status = "Active"
            };

            // Act
            var result = await repository.CreateAsync(machine);
            await context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("MACH-001", result.Code);
            Assert.Equal("CNC Lathe", result.Name);
        }

        [Fact]
        public async Task GetAllMachines_ReturnsAllMachines()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new MachineRepository(context);

            var machines = new[]
            {
                new Machine { Code = "M1", Name = "Machine 1", Type = "Type1", Location = "Loc1", Status = "Active" },
                new Machine { Code = "M2", Name = "Machine 2", Type = "Type2", Location = "Loc2", Status = "Active" },
                new Machine { Code = "M3", Name = "Machine 3", Type = "Type3", Location = "Loc3", Status = "Inactive" }
            };

            foreach (var machine in machines)
            {
                context.Machines.Add(machine);
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task UpdateMachine_ChangesStatus_Successfully()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new MachineRepository(context);

            var machine = new Machine
            {
                Code = "MACH-002",
                Name = "Mill",
                Type = "Milling",
                Location = "Floor 2",
                Status = "Active"
            };

            context.Machines.Add(machine);
            await context.SaveChangesAsync();

            // Act
            machine.Status = "Maintenance";
            await repository.UpdateAsync(machine);
            await context.SaveChangesAsync();

            var updated = await repository.GetByIdAsync(machine.Id);

            // Assert
            Assert.Equal("Maintenance", updated.Status);
        }
    }
}
