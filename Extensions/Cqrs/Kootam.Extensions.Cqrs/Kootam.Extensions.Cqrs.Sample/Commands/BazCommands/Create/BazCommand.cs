using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.BazCommands.Create;
public class CreateBazCommandHandler : RequestHandler<CreateBazCommand, string>
{
    public override async Task<Result<string>> Handle(CreateBazCommand command, CancellationToken cancellationToken)
    {
        return Ok(command.BazTitle);
    }
}

