using Kootam.Framework.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.Framework.Infrastructure.ValueConversions;

public class BusinessIdConversion : ValueConverter<BusinessId, Guid>
{
    public BusinessIdConversion() : base(c => c.Value, c => BusinessId.FromGuid(c))
    {

    }
}