using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class CreateNationalHolidayValidator : AbstractValidator<CreateNationalHolidayModel>
{
    public CreateNationalHolidayValidator()
    {
        RuleFor(x => x.Day)
            .InclusiveBetween(1, 31).WithMessage("O dia deve estar entre 1 e 31.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do feriado é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
    }
}

public sealed class UpdateNationalHolidayValidator : AbstractValidator<UpdateNationalHolidayModel>
{
    public UpdateNationalHolidayValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do feriado é obrigatório.");

        RuleFor(x => x.Day)
            .InclusiveBetween(1, 31).WithMessage("O dia deve estar entre 1 e 31.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do feriado é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
    }
}

public sealed class CreateStateHolidayValidator : AbstractValidator<CreateStateHolidayModel>
{
    public CreateStateHolidayValidator()
    {
        RuleFor(x => x.Day)
            .InclusiveBetween(1, 31).WithMessage("O dia deve estar entre 1 e 31.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do feriado é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");

        RuleFor(x => x.StateId)
            .NotEmpty().WithMessage("O estado é obrigatório.");
    }
}

public sealed class UpdateStateHolidayValidator : AbstractValidator<UpdateStateHolidayModel>
{
    public UpdateStateHolidayValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do feriado é obrigatório.");

        RuleFor(x => x.Day)
            .InclusiveBetween(1, 31).WithMessage("O dia deve estar entre 1 e 31.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do feriado é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
    }
}

public sealed class CreateMunicipalityHolidayValidator : AbstractValidator<CreateMunicipalityHolidayModel>
{
    public CreateMunicipalityHolidayValidator()
    {
        RuleFor(x => x.Day)
            .InclusiveBetween(1, 31).WithMessage("O dia deve estar entre 1 e 31.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do feriado é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");

        RuleFor(x => x.MunicipalityId)
            .NotEmpty().WithMessage("O município é obrigatório.");
    }
}

public sealed class UpdateMunicipalityHolidayValidator : AbstractValidator<UpdateMunicipalityHolidayModel>
{
    public UpdateMunicipalityHolidayValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O identificador do feriado é obrigatório.");

        RuleFor(x => x.Day)
            .InclusiveBetween(1, 31).WithMessage("O dia deve estar entre 1 e 31.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("O mês deve estar entre 1 e 12.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição do feriado é obrigatória.")
            .MaximumLength(100).WithMessage("A descrição deve ter no máximo 100 caracteres.");
    }
}
