namespace Kootam.Framework.Domain.Interfaces.Capability;

public interface IFullAuditedAggregateRoot<TKey> : ICreationAuditedObject<TKey>, IModificationAuditedObject<TKey>, ISoftDelete, IHasDeletionTime
{

}