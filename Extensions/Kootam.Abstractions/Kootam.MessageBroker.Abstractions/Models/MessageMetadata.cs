using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.MessageBroker.Abstractions.Models;

public class MessageMetadata
{
    public string MessageId { get; init; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public IDictionary<string, string> Headers { get; init; } = new Dictionary<string, string>();

}