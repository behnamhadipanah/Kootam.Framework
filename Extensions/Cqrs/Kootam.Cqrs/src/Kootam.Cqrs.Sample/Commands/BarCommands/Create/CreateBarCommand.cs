using Kootam.Cqrs.Abstractions.Commands;

namespace Kootam.Cqrs.Sample.Commands.BarCommands.Create;

public record CreateBarCommand(string Title):IRequest;