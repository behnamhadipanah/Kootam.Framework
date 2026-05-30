using Kootam.Extensions.Cqrs.Abstractions.Commands;
using Kootam.Extensions.Cqrs.Abstractions.Models;

namespace Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;

public record CreateFooCommand(string Title):IRequest<bool>;