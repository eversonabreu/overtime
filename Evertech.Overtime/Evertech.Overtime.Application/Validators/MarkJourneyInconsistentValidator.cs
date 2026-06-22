using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class MarkJourneyInconsistentValidator : AbstractValidator<MarkJourneyInconsistentModel>
{
    public MarkJourneyInconsistentValidator()
    {
        RuleFor(x => x.JourneyId)
            .NotEmpty().WithMessage("O identificador da jornada é obrigatório.");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("O motivo da inconsistência é obrigatório.")
            .MaximumLength(500).WithMessage("O motivo deve ter no máximo 500 caracteres.");
    }
}
