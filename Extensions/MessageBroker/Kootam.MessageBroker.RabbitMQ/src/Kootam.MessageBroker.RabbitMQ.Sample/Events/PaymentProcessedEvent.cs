namespace Kootam.MessageBroker.RabbitMQ.Sample.Events;

public sealed record PaymentProcessedEvent(
    Guid OrderId,
    decimal Amount,
    string Status,
    DateTime ProcessedAt);