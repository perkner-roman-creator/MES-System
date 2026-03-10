using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiniMES.Api.Data;
using MiniMES.Api.Services;
using MiniMES.Api.Models;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MiniMES.Tests.Services
{
    public class ExternalApiServiceTests
    {
        private MesDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<MesDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new MesDbContext(options);
        }

        private ILogger<ExternalApiService> CreateMockLogger()
        {
            return new Mock<ILogger<ExternalApiService>>().Object;
        }

        [Fact]
        public async Task ProcessWebhookEvent_WithValidWorkOrderCreated_CreatesWorkOrder()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = CreateMockLogger();
            var httpClientFactory = new Mock<IHttpClientFactory>();

            var service = new ExternalApiService(logger, context, httpClientFactory.Object);

            var payload = new
            {
                order_id = "SAP-001",
                product_code = "PROD-123",
                quantity = 100,
                due_date = DateTime.UtcNow.AddDays(7)
            };

            var webhookEvent = new WebhookEvent
            {
                Source = "SAP",
                EventType = "workorder_created",
                PayloadJson = JsonConvert.SerializeObject(payload),
                Status = "pending"
            };

            // Act
            var result = await service.ProcessWebhookEventAsync(webhookEvent);

            // Assert
            Assert.True(result);
            Assert.Equal("completed", webhookEvent.Status);

            var createdOrder = context.WorkOrders.FirstOrDefault(w => w.OrderNumber == "SAP-001");
            Assert.NotNull(createdOrder);
            Assert.Equal(100, createdOrder.Quantity);
        }

        [Fact]
        public async Task ProcessWebhookEvent_WithInvalidPayload_MarksFailed()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = CreateMockLogger();
            var httpClientFactory = new Mock<IHttpClientFactory>();

            var service = new ExternalApiService(logger, context, httpClientFactory.Object);

            var webhookEvent = new WebhookEvent
            {
                Source = "SAP",
                EventType = "unknown_event",
                PayloadJson = "{}",
                Status = "pending"
            };

            // Act
            var result = await service.ProcessWebhookEventAsync(webhookEvent);

            // Assert
            Assert.False(result);
            Assert.Equal("pending", webhookEvent.Status); // Retry možný
        }

        [Fact]
        public async Task GetPendingEvents_ReturnsPendingEventsUnderRetryLimit()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = CreateMockLogger();
            var httpClientFactory = new Mock<IHttpClientFactory>();

            context.WebhookEvents.Add(new WebhookEvent
            {
                Source = "SAP",
                EventType = "test",
                Status = "pending",
                RetryCount = 0
            });

            context.WebhookEvents.Add(new WebhookEvent
            {
                Source = "SAP",
                EventType = "test",
                Status = "pending",
                RetryCount = 5 // Over limit
            });

            context.WebhookEvents.Add(new WebhookEvent
            {
                Source = "SAP",
                EventType = "test",
                Status = "completed",
                RetryCount = 0
            });

            await context.SaveChangesAsync();

            var service = new ExternalApiService(logger, context, httpClientFactory.Object);

            // Act
            var pendingEvents = await service.GetPendingEventsAsync();

            // Assert
            Assert.Single(pendingEvents);
            Assert.Equal(0, pendingEvents[0].RetryCount);
        }

        [Fact]
        public async Task RetryFailedEvents_ProcessesAllPendingEvents()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var logger = CreateMockLogger();
            var httpClientFactory = new Mock<IHttpClientFactory>();

            for (int i = 0; i < 3; i++)
            {
                context.WebhookEvents.Add(new WebhookEvent
                {
                    Source = "SAP",
                    EventType = "unknown",
                    Status = "pending",
                    RetryCount = 0,
                    PayloadJson = "{}"
                });
            }

            await context.SaveChangesAsync();

            var service = new ExternalApiService(logger, context, httpClientFactory.Object);

            // Act
            var result = await service.RetryFailedEventsAsync();

            // Assert
            Assert.True(result);
        }
    }
}
