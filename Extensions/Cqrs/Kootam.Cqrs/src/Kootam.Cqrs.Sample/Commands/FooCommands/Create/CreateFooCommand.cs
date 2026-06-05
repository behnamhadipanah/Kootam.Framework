using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Sample.Commands.FooCommands.Create;

public record CreateFooCommand(string Title):IRequest<bool>;