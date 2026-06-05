using Kootam.Cqrs.Abstractions.Commands;

namespace Kootam.Cqrs.Sample.Commands.QuxCommands.Create;

public record CreateQuxCommand(string name) : IRequest;


