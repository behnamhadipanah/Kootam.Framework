using Kootam.Extensions.Cqrs.Abstractions.Commands;


namespace Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;

public record CreateBarCommand(string Title):IRequest;