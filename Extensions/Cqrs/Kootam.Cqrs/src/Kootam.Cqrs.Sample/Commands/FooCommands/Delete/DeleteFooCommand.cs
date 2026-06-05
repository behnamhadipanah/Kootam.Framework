using Kootam.Cqrs.Abstractions.Commands;

namespace Kootam.Cqrs.Sample.Commands.FooCommands.Delete;

public record DeleteFooCommand(long Id):IRequest<bool>;