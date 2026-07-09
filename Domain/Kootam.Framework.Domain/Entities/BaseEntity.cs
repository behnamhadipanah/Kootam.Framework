using Kootam.Framework.Domain.Contracts.Markers;
using Kootam.Framework.Domain.ValueObjects;

namespace Kootam.Framework.Domain.Entities;

public abstract class BaseEntity<TKey> : IAuditableEntity
where TKey : struct
{
    public TKey Id { get; protected set; }
    public BusinessId BusinessId { get; protected set; } = BusinessId.FromGuid(Guid.NewGuid());
    protected BaseEntity()
    {

    }

    #region Equality Check
    public bool Equals(BaseEntity<TKey>? other) => this == other;
    public override bool Equals(object? obj) =>
        obj is BaseEntity<TKey> otherObject && Id.Equals(otherObject.Id);

    public override int GetHashCode() => Id.GetHashCode();
    public static bool operator ==(BaseEntity<TKey> left, BaseEntity<TKey> right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity<TKey> left, BaseEntity<TKey> right)
        => !(right == left);

    #endregion
}
public abstract class BaseEntity : BaseEntity<long>
{

}
