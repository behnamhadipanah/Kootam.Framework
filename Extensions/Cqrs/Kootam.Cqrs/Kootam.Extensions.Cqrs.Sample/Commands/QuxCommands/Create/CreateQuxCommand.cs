using Kootam.Extensions.Cqrs.Abstractions.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.QuxCommands.Create;

public record CreateQuxCommand(string name) : IRequest;


