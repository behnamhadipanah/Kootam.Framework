using Kootam.Framework.Domain.Events;

namespace Kootam.Framework.Domain.Entities;

public interface IAggregateRoot
{
    void ClearEvents();
    IEnumerable<IDomainEvent> GetEvents();
}
