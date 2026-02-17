using System;
using System.Collections.Generic;
using System.Text;

namespace Kootam.Framework.Application.Queries.Grid;

public readonly record struct GridFilterResponse<T>(
    IReadOnlyList<T> Data,
    int TotalCount
);
