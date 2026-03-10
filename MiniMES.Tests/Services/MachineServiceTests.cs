using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using MiniMES.Api.Models;
using MiniMES.Api.Services;
using MiniMES.Api.Repositories;

namespace MiniMES.Tests.Services
{
    [TestClass]
    public class MachineServiceTests
    {
        private Mock<IMachineRepository> _mockRepository;
        private MachineService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IMachineRepository>();
            _service = new MachineService(_mockRepository.Object);
        }

        [TestMethod]
        public void GetAllMachines_ReturnsAllMachines()
        {
            // Arrange
            var machines = new[]
            {
                new Machine { Id = 1, Name = "CNC Freza 1", MachineCode = "MACHINE_001", IsActive = true },
                new Machine { Id = 2, Name = "CNC Soustruh 1", MachineCode = "MACHINE_002", IsActive = true }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(machines.AsQueryable());

            // Act
            var result = _service.GetAllMachines();

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.All(m => m.IsActive));
        }

        [TestMethod]
        public void GetMachineById_WithValidId_ReturnsMachine()
        {
            // Arrange
            var machineId = 1;
            var machine = new Machine { Id = machineId, Name = "CNC Freza 1", MachineCode = "MACHINE_001" };
            _mockRepository.Setup(r => r.GetById(machineId)).Returns(machine);

            // Act
            var result = _service.GetMachineById(machineId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("CNC Freza 1", result.Name);
        }

        [TestMethod]
        public void GetMachineById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetById(It.IsAny<int>())).Returns((Machine)null);

            // Act
            var result = _service.GetMachineById(999);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateMachine_WithNullName_ThrowsException()
        {
            // Act
            _service.CreateMachine(null, "MACHINE_001", "CNC");
        }

        [TestMethod]
        public void CreateMachine_WithValidData_CreatesSuccessfully()
        {
            // Arrange
            var name = "Novy stroj";
            var code = "MACHINE_NEW";
            var type = "CNC";

            // Act
            _service.CreateMachine(name, code, type);

            // Assert
            _mockRepository.Verify(r => r.Add(It.IsAny<Machine>()), Times.Once);
            _mockRepository.Verify(r => r.Save(), Times.Once);
        }
    }
}
