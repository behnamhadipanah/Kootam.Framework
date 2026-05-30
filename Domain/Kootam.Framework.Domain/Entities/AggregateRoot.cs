using Kootam.Framework.Domain.Events;
using System.Reflection;

namespace Kootam.Framework.Domain.Entities;

public abstract class AggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot 
    where TKey : struct, IComparable, IComparable<TKey>, IConvertible, IEquatable<TKey>, IFormattable
{
    private readonly List<IDomainEvent> _events;
    protected AggregateRoot() => _events = new();
    public AggregateRoot(IEnumerable<IDomainEvent> events)
    {
        if (events == null || !events.Any()) return;
        foreach (var @event in events)
        {
            Mutate(@event);
        }
    }
    protected void Apply(IDomainEvent @event)
    {
        Mutate(@event);
        AddEvent(@event);
    }
    protected void AddEvent(IDomainEvent @event) => _events.Add(@event);

    private void Mutate(IDomainEvent @event)
    {
        var onMethod = this.GetType().GetMethod("On", BindingFlags.Instance | BindingFlags.NonPublic, [@event.GetType()]);
        onMethod.Invoke(this, new[] { @event });
    }
    public void ClearEvents() => _events.Clear();


    public IEnumerable<IDomainEvent> GetEvents() => _events.AsEnumerable();

}
public abstract class AggregateRoot : AggregateRoot<long>
{

}