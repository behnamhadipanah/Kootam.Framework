using Kootam.Extensions.Cqrs.Abstractions.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Update;

public record UpdateFooCommand(long Id,string Title):IRequest<bool>;