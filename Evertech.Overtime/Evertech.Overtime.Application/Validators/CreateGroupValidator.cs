using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class CreateGroupValidator : AbstractValidator<CreateGroupModel>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do grupo é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do grupo deve ter no máximo 150 caracteres.");

        RuleFor(x => x.LeaderPersonId)
            .NotEmpty().WithMessage("O líder inicial do grupo é obrigatório.");
    }
}
