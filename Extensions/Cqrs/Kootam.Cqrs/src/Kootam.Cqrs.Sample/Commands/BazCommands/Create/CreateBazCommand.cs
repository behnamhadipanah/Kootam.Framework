using Kootam.Cqrs.Abstractions.Commands;

namespace Kootam.Cqrs.Sample.Commands.BazCommands.Create;

public record CreateBazCommand(string BazTitle) : IRequest<string>;

