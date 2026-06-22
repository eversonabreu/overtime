using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class UpdateGroupValidator : AbstractValidator<UpdateGroupModel>
{
    public UpdateGroupValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do grupo é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do grupo é obrigatório.")
            .MaximumLength(150).WithMessage("O nome do grupo deve ter no máximo 150 caracteres.");
    }
}
