using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Commands;

namespace Kootam.Cqrs.Sample.Commands.QuxCommands.Create;

public class CreateQuxCommandHandler : RequestHandler<CreateQuxCommand>
{
    public override async Task<Result> Handle(CreateQuxCommand command, CancellationToken cancellationToken)
    {
        return Ok();
    }
}


