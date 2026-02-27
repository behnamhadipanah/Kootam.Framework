namespace Kootam.Framework.Domain.Interfaces.Capability
{
    public interface IMultiTenant<TKey>
    {
        TKey TenantId { get; }
    }
}
