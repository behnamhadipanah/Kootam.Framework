using Kootam.Extensions.Cqrs.Abstractions.Queries;

namespace Kootam.Extensions.Cqrs.Sample.Queries.FooQueries;

public record FooViewModel(string BarName);


public record GetFooListQuery() : IQuery<List<FooViewModel>>;

