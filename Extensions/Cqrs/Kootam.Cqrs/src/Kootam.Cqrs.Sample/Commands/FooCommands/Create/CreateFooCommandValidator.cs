using FluentValidation;
using Kootam.Cqrs.Abstractions.Commands;

namespace Kootam.Cqrs.Sample.Commands.FooCommands.Create;


public class CreateFooCommandValidator : AbstractValidator<CreateFooCommand>
{
    public CreateFooCommandValidator()
    {
        RuleFor(c=>c.Title).NotEmpty().WithMessage("Title cannot be empty");
    }
}