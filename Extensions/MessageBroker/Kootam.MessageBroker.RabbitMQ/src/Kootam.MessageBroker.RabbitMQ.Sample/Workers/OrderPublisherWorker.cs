using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.Abstractions.Models;
using Kootam.MessageBroker.RabbitMQ.Sample.Events;

namespace Kootam.MessageBroker.RabbitMQ.Sample.Workers;

/// <summary>
/// Example: publishes an OrderCreatedEvent every 5 seconds.
/// Inject IMessagePublisher when you only need to publish.
/// </summary>
public sealed class OrderPublisherWorker : BackgroundService
{
    private readonly IMessagePublisher _publisher;
    private readonly ILogger<OrderPublisherWorker> _logger;

    private const string QueueName = "orders.created";

    public OrderPublisherWorker(IMessagePublisher publisher, ILogger<OrderPublisherWorker> logger)
    {
        _publisher = publisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderPublisherWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var order = new OrderCreatedEvent(
                OrderId: Guid.NewGuid(),
                CustomerName: "Kootam Test",
                TotalAmount: Random.Shared.Next(100, 10_000),
                CreatedAt: DateTime.UtcNow);

            // Simple publish
            await _publisher.PublishAsync(order, QueueName, cancellationToken: stoppingToken);

            // Publish with metadata (CorrelationId, custom headers)
            var envelope = new MessageEnvelope<OrderCreatedEvent>
            {
                Payload = order,
                Metadata = new MessageMetadata
                {
                    CorrelationId = Guid.NewGuid().ToString(),
                    Source = "OrderService",
                    Headers = { ["x-priority"] = "high" }
                }
            };

            await _publisher.PublishAsync(envelope, QueueName, cancellationToken: stoppingToken);

            _logger.LogInformation("Published order {OrderId}", order.OrderId);
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}