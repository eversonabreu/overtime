using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class CreateJourneyValidator : AbstractValidator<CreateJourneyModel>
{
    private const int MaxJourneyMinutes = 480;

    public CreateJourneyValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("O colaborador é obrigatório.");

        RuleFor(x => x.CheckIn)
            .NotEmpty().WithMessage("A entrada (check-in) é obrigatória.");

        RuleFor(x => x.CheckOut)
            .NotEmpty().WithMessage("A saída (check-out) é obrigatória.")
            .GreaterThan(x => x.CheckIn).WithMessage("A saída deve ser posterior à entrada.");

        RuleFor(x => x)
            .Must(x => x.CheckIn.Month == x.CheckOut.Month && x.CheckIn.Year == x.CheckOut.Year)
            .WithMessage("O check-in e o check-out devem pertencer ao mesmo mês.")
            .Must(x => (int)(x.CheckOut - x.CheckIn).TotalMinutes <= MaxJourneyMinutes)
            .WithMessage($"A jornada não pode ultrapassar {MaxJourneyMinutes} minutos (8 horas).")
            .Must(x => x.CheckIn.Month == DateTimeOffset.UtcNow.Month && x.CheckIn.Year == DateTimeOffset.UtcNow.Year)
            .WithMessage("Somente é permitido lançar jornadas do mês corrente.");
    }
}
