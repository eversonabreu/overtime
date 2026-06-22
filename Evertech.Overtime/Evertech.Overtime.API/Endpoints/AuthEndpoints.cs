using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder AddAuthEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/auth").WithTags("Auth").AllowAnonymous();

        group.MapPost("/login", async (
            LoginModel model,
            IAuthService authService,
            IValidator<LoginModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var token = await authService.LoginAsync(model, cancellationToken);
            return Results.Ok(new { data = token });
        });

        group.MapPost("/refresh", async (
            RefreshTokenModel model,
            IAuthService authService,
            IValidator<RefreshTokenModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var token = await authService.RefreshAsync(model, cancellationToken);
            return Results.Ok(new { data = token });
        });

        group.MapPost("/revoke", async (
            RevokeTokenModel model,
            IAuthService authService,
            IValidator<RevokeTokenModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await authService.RevokeAsync(model, cancellationToken);
            return Results.NoContent();
        });

        return builder;
    }
}
