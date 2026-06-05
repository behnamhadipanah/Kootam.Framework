namespace Kootam.MessageBroker.RabbitMQ.Sample.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    string CustomerName,
    decimal TotalAmount,
    DateTime CreatedAt);