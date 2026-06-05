using Kootam.Cqrs.Abstractions.Queries;

namespace Kootam.Cqrs.Sample.Queries.BarQueries;


public record BarViewModel(string BarName);


public record BarGetTitleQuery(string Name) : IQuery<List<BarViewModel>>;
