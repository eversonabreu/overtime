using Evertech.Overtime.Application.Models;
using FluentValidation;

namespace Evertech.Overtime.Application.Validators;

public sealed class LoginValidator : AbstractValidator<LoginModel>
{
    public LoginValidator()
    {
        RuleFor(x => x.Credential)
            .NotEmpty().WithMessage("O e-mail ou matrícula é obrigatório.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.");
    }
}

public sealed class RefreshTokenValidator : AbstractValidator<RefreshTokenModel>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O refresh token é obrigatório.");
    }
}

public sealed class RevokeTokenValidator : AbstractValidator<RevokeTokenModel>
{
    public RevokeTokenValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("O refresh token é obrigatório.");
    }
}