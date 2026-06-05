using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.QuxCommands.Create;

public class CreateQuxCommandHandler : RequestHandler<CreateQuxCommand>
{
    public override async Task<Result> Handle(CreateQuxCommand command, CancellationToken cancellationToken)
    {
        return Ok();
    }
}


