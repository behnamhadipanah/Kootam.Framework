using Kootam.Cqrs.Abstractions.Queries;

namespace Kootam.Cqrs.Sample.Queries.FooQueries;

public record FooViewModel(string BarName);


public record GetFooListQuery() : IQuery<List<FooViewModel>>;

