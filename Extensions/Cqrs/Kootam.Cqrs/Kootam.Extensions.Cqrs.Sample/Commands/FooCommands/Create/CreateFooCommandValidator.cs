using FluentValidation;
using Kootam.Extensions.Cqrs.Abstractions.Commands;

namespace Kootam.Extensions.Cqrs.Sample.Commands.FooCommands.Create;


public class CreateFooCommandValidator : AbstractValidator<CreateFooCommand>
{
    public CreateFooCommandValidator()
    {
        RuleFor(c=>c.Title).NotEmpty().WithMessage("Title cannot be empty");
    }
}