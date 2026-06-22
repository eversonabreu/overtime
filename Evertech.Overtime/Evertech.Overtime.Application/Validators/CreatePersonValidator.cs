using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class CreatePersonValidator : AbstractValidator<CreatePersonModel>
{
    public CreatePersonValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(150).WithMessage("O nome deve ter no máximo 150 caracteres.");

        RuleFor(x => x.Registration)
            .NotEmpty().WithMessage("A matrícula é obrigatória.")
            .MaximumLength(20).WithMessage("A matrícula deve ter no máximo 20 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("O e-mail informado não é válido.");

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0).WithMessage("O valor/hora deve ser maior que zero.");

        RuleFor(x => x.MunicipalityId)
            .NotEmpty().WithMessage("O município é obrigatório.");
    }
}
