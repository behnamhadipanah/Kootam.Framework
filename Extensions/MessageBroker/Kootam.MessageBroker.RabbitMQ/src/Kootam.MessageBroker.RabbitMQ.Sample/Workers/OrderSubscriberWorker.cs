using Kootam.MessageBroker.Abstractions.Contracts;
using Kootam.MessageBroker.Abstractions.Models;
using Kootam.MessageBroker.RabbitMQ.Sample.Events;

namespace Kootam.MessageBroker.RabbitMQ.Sample.Workers;

/// <summary>
/// Example: subscribes to OrderCreatedEvent.
/// Inject IMessageSubscriber when you only need to consume.
/// </summary>
public sealed class OrderSubscriberWorker : BackgroundService
{
    private readonly IMessageSubscriber _subscriber;
    private readonly ILogger<OrderSubscriberWorker> _logger;

    private const string QueueName = "orders.created";

    public OrderSubscriberWorker(IMessageSubscriber subscriber, ILogger<OrderSubscriberWorker> logger)
    {
        _subscriber = subscriber;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _subscriber.SubscribeAsync<OrderCreatedEvent>(
            queueName: QueueName,
            handler: HandleOrderCreatedAsync,
            options: new SubscribeOptions
            {
                PrefetchCount = 10,
                AutoAck = false,
                Durable = true
            },
            cancellationToken: stoppingToken);

        // Keep the worker alive; SubscribeAsync registers the consumer and returns
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task HandleOrderCreatedAsync(MessageContext<OrderCreatedEvent> ctx)
    {
        var order = ctx.Message;
        _logger.LogInformation(
            "Received order {OrderId} from {Customer} — Amount: {Amount} | CorrelationId: {CorrelationId}",
            order.OrderId, order.CustomerName, order.TotalAmount, ctx.Metadata.CorrelationId);

        // Business logic here...

        return Task.CompletedTask;
    }
}