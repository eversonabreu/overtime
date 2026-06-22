using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class UpdateCompensatoryConversionValidator : AbstractValidator<UpdateCompensatoryConversionModel>
{
    public UpdateCompensatoryConversionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador da conversão é obrigatório.");

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
