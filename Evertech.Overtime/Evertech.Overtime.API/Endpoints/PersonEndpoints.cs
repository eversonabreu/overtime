using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class PersonEndpoints
{
    public static IEndpointRouteBuilder AddPersonEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/persons").WithTags("Persons").RequireAuthorization();

        group.MapPost("/bootstrap", async (
            CreateFirstPersonModel model,
            IPersonService personService,
            IValidator<CreateFirstPersonModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await personService.CreateFirstAsync(model, cancellationToken);
            return id is null
                ? Results.NoContent()
                : Results.Ok(new { data = new { id } });
        });

        group.MapPost("/", async (
            CreatePersonModel model,
            IPersonService personService,
            IValidator<CreatePersonModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await personService.CreateAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IPersonService personService,
            CancellationToken cancellationToken) =>
        {
            var person = await personService.GetByIdAsync(id, cancellationToken);
            return person is null
                ? Results.NotFound()
                : Results.Ok(new { data = person });
        });

        group.MapPut("/", async (
            UpdatePersonModel model,
            IPersonService personService,
            IValidator<UpdatePersonModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await personService.UpdateAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        group.MapPatch("/change-password", async (
            ChangePasswordModel model,
            IPersonService personService,
            IValidator<ChangePasswordModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await personService.ChangePasswordAsync(model, cancellationToken);
            return Results.NoContent();
        });

        group.MapPatch("/request-password-reset", async (
            RequestPasswordResetModel model,
            IPersonService personService,
            IValidator<RequestPasswordResetModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await personService.RequestPasswordResetAsync(model, cancellationToken);
            return Results.NoContent();
        });

        group.MapPatch("/reset-password", async (
            ResetPasswordModel model,
            IPersonService personService,
            IValidator<ResetPasswordModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await personService.ResetPasswordAsync(model, cancellationToken);
            return Results.NoContent();
        });

        return builder;
    }
}
