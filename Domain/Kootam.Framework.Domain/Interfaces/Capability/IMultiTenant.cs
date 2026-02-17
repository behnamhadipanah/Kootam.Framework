using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.Framework.Domain.Interfaces.Capability
{
    public interface IMultiTenant<TKey>
    {
        TKey TenantId {  get; }
    }
}
