namespace Kootam.Framework.Domain.Contracts.Capability
{
    public interface IMultiTenant<TKey>
    {
        TKey TenantId { get; }
    }
}
