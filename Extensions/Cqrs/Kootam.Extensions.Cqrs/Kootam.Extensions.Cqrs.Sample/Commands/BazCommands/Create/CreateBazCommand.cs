using Kootam.Extensions.Cqrs.Abstractions.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.BazCommands.Create;

public record CreateBazCommand(string BazTitle) : IRequest<string>;

