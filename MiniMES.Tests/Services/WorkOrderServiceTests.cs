using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using MiniMES.Api.Models;
using MiniMES.Api.Services;
using MiniMES.Api.Repositories;

namespace MiniMES.Tests.Services
{
    [TestClass]
    public class WorkOrderServiceTests
    {
        private Mock<IWorkOrderRepository> _mockRepository;
        private WorkOrderService _service;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IWorkOrderRepository>();
            _service = new WorkOrderService(_mockRepository.Object);
        }

        [TestMethod]
        public void GetActiveWorkOrders_ReturnsOnlyActiveOrders()
        {
            // Arrange
            var orders = new[]
            {
                new WorkOrder { Id = 1, OrderNumber = "WO-001", Status = WorkOrderStatus.InProgress },
                new WorkOrder { Id = 2, OrderNumber = "WO-002", Status = WorkOrderStatus.Completed },
                new WorkOrder { Id = 3, OrderNumber = "WO-003", Status = WorkOrderStatus.Pending }
            };
            _mockRepository.Setup(r => r.GetAll()).Returns(orders.AsQueryable());

            // Act
            var result = _service.GetActiveWorkOrders();

            // Assert
            Assert.IsTrue(result.All(o => o.Status != WorkOrderStatus.Completed));
        }

        [TestMethod]
        public void StartWorkOrder_SetsStatusToInProgress()
        {
            // Arrange
            var orderId = 1;
            var order = new WorkOrder { Id = orderId, OrderNumber = "WO-001", Status = WorkOrderStatus.Pending };
            _mockRepository.Setup(r => r.GetById(orderId)).Returns(order);

            // Act
            _service.StartWorkOrder(orderId);

            // Assert
            Assert.AreEqual(WorkOrderStatus.InProgress, order.Status);
            _mockRepository.Verify(r => r.Save(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CreateWorkOrder_WithNullOrderNumber_ThrowsException()
        {
            // Act
            _service.CreateWorkOrder(null, "Product", 10);
        }

        [TestMethod]
        public void CreateWorkOrder_WithValidData_CreatesSuccessfully()
        {
            // Arrange
            var orderNumber = "WO-NEW-001";
            var productName = "Test Product";
            var quantity = 100;

            // Act
            _service.CreateWorkOrder(orderNumber, productName, quantity);

            // Assert
            _mockRepository.Verify(r => r.Add(It.IsAny<WorkOrder>()), Times.Once);
            _mockRepository.Verify(r => r.Save(), Times.Once);
        }
    }
}
