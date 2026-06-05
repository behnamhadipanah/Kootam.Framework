using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Abstractions.Queries;

namespace Kootam.Cqrs.Sample.Queries.FooQueries;

public class GetFooListQueryHandler : IQueryHandler<GetFooListQuery, List<FooViewModel>>
{
    public async Task<Result<List<FooViewModel>>> Handle(GetFooListQuery query, CancellationToken cancellationToken = default)
    {
        var testList = new List<FooViewModel>()
        {
            new FooViewModel("test1"),new FooViewModel("test2")
        };
        return Result<List<FooViewModel>>.Success(testList);
    }
}

//public class BarGetTitleQueryHandler : QueryHandler<BarGetTitleQuery, List<BarViewModel>>
//{

//    public override async Task<Result<List<BarViewModel>>> Handle(BarGetTitleQuery query, CancellationToken cancellationToken)
//    {
//        var testList = new List<BarViewModel>()
//        {
//            new BarViewModel("test1"),new BarViewModel("test2")
//        };
//        return Ok(testList);
//    }
//}