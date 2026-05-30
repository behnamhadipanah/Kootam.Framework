using FluentValidation;
using Kootam.Extensions.Cqrs.Abstractions.Commands;
using Kootam.Extensions.Cqrs.Abstractions.Models;
using Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;

namespace Kootam.Extensions.Cqrs.Sample.Commands.Create;

public class CreateBarCommandHandler : IRequestHandler<CreateBarCommand>
{
    public async Task<Result> Handle(CreateBarCommand command, CancellationToken cancellationToken = default)
    {
        
        return Result.Success();
    }
}
public class CreateBarCommandValidator : AbstractValidator<CreateBarCommand>
{
    public CreateBarCommandValidator()
    {
        
    }
}