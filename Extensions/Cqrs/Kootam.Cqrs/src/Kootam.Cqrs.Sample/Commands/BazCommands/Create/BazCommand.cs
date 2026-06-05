using Kootam.Cqrs.Abstractions.Models;
using Kootam.Cqrs.Commands;

namespace Kootam.Cqrs.Sample.Commands.BazCommands.Create;
public class CreateBazCommandHandler : RequestHandler<CreateBazCommand, string>
{
    public override async Task<Result<string>> Handle(CreateBazCommand command, CancellationToken cancellationToken)
    {
        return Ok(command.BazTitle);
    }
}

