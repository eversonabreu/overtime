using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Domain.Enums;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class CreateCompensatoryConversionValidator : AbstractValidator<CreateCompensatoryConversionModel>
{
    public CreateCompensatoryConversionValidator()
    {
        RuleFor(x => x.PersonId)
            .NotEmpty().WithMessage("O colaborador é obrigatório.");

        RuleFor(x => x.Minutes)
            .GreaterThan(0).WithMessage("A quantidade de minutos deve ser maior que zero.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("O tipo de conversão informado é inválido.");

        RuleFor(x => x.ConversionDate)
            .Must(date => date.Month == DateOnly.FromDateTime(DateTime.UtcNow).Month &&
                          date.Year == DateOnly.FromDateTime(DateTime.UtcNow).Year)
            .WithMessage("A data de conversão deve pertencer ao mês corrente.");
    }
}
