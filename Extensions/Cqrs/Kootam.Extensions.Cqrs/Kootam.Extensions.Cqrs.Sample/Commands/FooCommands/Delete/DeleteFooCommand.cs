using Kootam.Extensions.Cqrs.Abstractions.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Delete;

public record DeleteFooCommand(long Id):IRequest<bool>;