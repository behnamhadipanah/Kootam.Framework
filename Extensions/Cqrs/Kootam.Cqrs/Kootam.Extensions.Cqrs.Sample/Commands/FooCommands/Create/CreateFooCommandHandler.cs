using Kootam.Extensions.Cqrs.Abstractions.Commands;
using Kootam.Extensions.Cqrs.Abstractions.Models;

namespace Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;

public class CreateFooCommandHandler : IRequestHandler<CreateFooCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateFooCommand command, CancellationToken cancellationToken)
    {

        return  Result<bool>.Success(data:true,message:command.Title);
    }
}
