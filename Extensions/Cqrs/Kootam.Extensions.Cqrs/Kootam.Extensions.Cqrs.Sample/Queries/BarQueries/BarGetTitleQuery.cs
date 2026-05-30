using Kootam.Extensions.Cqrs.Abstractions.Queries;

namespace Kootam.Extensions.Cqrs.Sample.Queries.BarQueries;


public record BarViewModel(string BarName);


public record BarGetTitleQuery(string Name) : IQuery<List<BarViewModel>>;
