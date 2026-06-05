using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Sample.Commands.FooCommands.Create;

public class CreateFooCommandHandler : IRequestHandler<CreateFooCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateFooCommand command, CancellationToken cancellationToken)
    {

        return  Result<bool>.Success(data:true,message:command.Title);
    }
}
