using Kootam.Cqrs.Abstractions.Commands;

namespace Kootam.Cqrs.Sample.Commands.FooCommands.Update;

public record UpdateFooCommand(long Id,string Title):IRequest<bool>;