namespace Kootam.Framework.Domain.Interfaces.Capability;

public interface IModificationAuditedObject<TKey>
{
    DateTime? LastModificationTime { get; }
    TKey? LastModifierId { get; }
    void SetLastModification(DateTime dateTime);
    void SetLastModifier(TKey userId);
}


