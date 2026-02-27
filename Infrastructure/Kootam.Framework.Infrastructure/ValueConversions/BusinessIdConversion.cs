using Kootam.Framework.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kootam.Framework.Infrastructure.ValueConversions;

public class BusinessIdConversion : ValueConverter<BusinessId, Guid>
{
    public BusinessIdConversion() : base(c => c.Value, c => BusinessId.FromGuid(c))
    {

    }
}