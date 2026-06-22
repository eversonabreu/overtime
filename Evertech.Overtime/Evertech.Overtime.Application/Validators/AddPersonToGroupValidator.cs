using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class AddPersonToGroupValidator : AbstractValidator<AddPersonToGroupModel>
{
    public AddPersonToGroupValidator()
    {
        RuleFor(x => x.GroupId)
            .NotEmpty().WithMessage("O identificador do grupo é obrigatório.");

        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("O identificador da pessoa é obrigatório.");
    }
}
