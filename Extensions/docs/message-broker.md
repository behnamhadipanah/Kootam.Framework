# Message Broker Extension

Publish/subscribe messaging with a transport-agnostic abstraction. Currently **RabbitMQ is fully implemented**; Kafka and Azure Service Bus packages are scaffolded.

| Package | Status | Sample |
|---------|--------|--------|
| `Kootam.MessageBroker.Abstractions` | Contracts | — |
| `Kootam.MessageBroker.RabbitMQ` | Implemented | `Extensions/MessageBroker/Kootam.MessageBroker.RabbitMQ/src/Kootam.MessageBroker.RabbitMQ.Sample/` |
| `Kootam.MessageBroker.Kafka` | Scaffold (no source yet) | — |
| `Kootam.MessageBroker.AzureServiceBus` | Scaffold (no source yet) | — |

---

## Abstractions

| Type | Role |
|------|------|
| `IMessagePublisher` | Publish to a queue |
| `IMessageSubscriber` | Subscribe with handler |
| `IMessageBus` | Combined publish + subscribe |
| `IMessageBrokerConnection` | Connection management |
| `MessageEnvelope<T>` | Payload + metadata |
| `MessageContext<T>` | Received message context |
| `MessageMetadata` | CorrelationId, Source, Headers |
| `SubscribeOptions` | PrefetchCount, AutoAck, Durable |
| `PublishOptions` | Publish settings |

---

## RabbitMQ Registration

### Via configuration

```csharp
using Kootam.MessageBroker.RabbitMQ.DependencyInjection;

builder.Services.AddRabbitMQ(builder.Configuration);
```

**appsettings.json:**

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "ClientProvidedName": "MyService",
    "RetryCount": 5,
    "RetryBaseDelaySeconds": 2
  }
}
```

### Inline

```csharp
builder.Services.AddRabbitMQ(opt =>
{
    opt.HostName = "localhost";
    opt.UserName = "guest";
    opt.Password = "guest";
});
```

---

## Publishing

```csharp
public class OrderService(IMessagePublisher publisher)
{
    private const string QueueName = "orders.created";

    public async Task CreateOrderAsync(CreateOrderDto dto, CancellationToken ct)
    {
        var order = new OrderCreatedEvent(
            OrderId: Guid.NewGuid(),
            CustomerName: dto.CustomerName,
            TotalAmount: dto.TotalAmount,
            CreatedAt: DateTime.UtcNow);

        // Simple publish
        await publisher.PublishAsync(order, QueueName, cancellationToken: ct);

        // With metadata
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
        await publisher.PublishAsync(envelope, QueueName, cancellationToken: ct);
    }
}
```

---

## Subscribing (BackgroundService)

```csharp
public sealed class OrderSubscriberWorker : BackgroundService
{
    private readonly IMessageSubscriber _subscriber;
    private const string QueueName = "orders.created";

    public OrderSubscriberWorker(IMessageSubscriber subscriber) => _subscriber = subscriber;

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

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private Task HandleOrderCreatedAsync(MessageContext<OrderCreatedEvent> ctx)
    {
        var order = ctx.Message;
        // business logic...
        return Task.CompletedTask;
    }
}
```

Register in DI:

```csharp
services.AddRabbitMQ(configuration);
services.AddHostedService<OrderSubscriberWorker>();
```

---

## Event Design

Define events as immutable records in a shared contracts project or Domain:

```csharp
public record OrderCreatedEvent(
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt);
```

Use past-tense names: `{Entity}{Action}Event`.

---

## Host Types

| Host | Registration |
|------|--------------|
| ASP.NET Core Web API | `builder.Services.AddRabbitMQ(...)` |
| Worker Service | `Host.CreateDefaultBuilder().ConfigureServices(...)` |

Sample uses `Host.CreateDefaultBuilder` with `OrderPublisherWorker` and `OrderSubscriberWorker`.

---

## Agent Rules

- Use `IMessagePublisher` / `IMessageSubscriber` — not RabbitMQ client types directly.
- Event classes are plain records — no base class required.
- Set `CorrelationId` in metadata for traceability across services.
- Use `Durable = true` for production queues.
- Handlers should be idempotent — messages may be redelivered.
- Do not use Kafka/Azure packages until source is implemented — use RabbitMQ.
