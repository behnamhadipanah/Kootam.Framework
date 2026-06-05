using FluentValidation;
using Kootam.Cqrs.Abstractions.Commands;
using Kootam.Cqrs.Abstractions.Models;

namespace Kootam.Cqrs.Sample.Commands.BarCommands.Create;

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