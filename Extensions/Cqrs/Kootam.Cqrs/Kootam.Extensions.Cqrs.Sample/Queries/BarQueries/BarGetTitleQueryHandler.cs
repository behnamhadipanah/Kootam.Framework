using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Queries;

namespace Kootam.Extensions.Cqrs.Sample.Queries.BarQueries;

public class BarGetTitleQueryHandler : QueryHandler<BarGetTitleQuery, List<BarViewModel>>
{

    public override async Task<Result<List<BarViewModel>>> Handle(BarGetTitleQuery query, CancellationToken cancellationToken)
    {
        var testList = new List<BarViewModel>()
        {
            new BarViewModel("test1"),new BarViewModel("test2")
        };
        return Ok(testList);
    }
}