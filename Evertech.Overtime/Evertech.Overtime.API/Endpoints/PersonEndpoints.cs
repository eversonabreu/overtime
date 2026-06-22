using System.Security.Claims;
using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using Evertech.Overtime.API.Extensions;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class PersonEndpoints
{
    public static IEndpointRouteBuilder AddPersonEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/persons").WithTags("Persons");

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
        }).AllowAnonymous();

        group.MapPost("/", async (
            CreatePersonModel model,
            ClaimsPrincipal user,
            IPersonService personService,
            IValidator<CreatePersonModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await personService.CreateAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        }).RequireAuthorization();

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

        group.MapGet("/", async (
            IPersonService personService,
            CancellationToken cancellationToken) =>
        {
            var persons = await personService.GetActiveAsync(cancellationToken);
            return Results.Ok(new { data = persons });
        }).RequireAuthorization();

        group.MapPut("/", async (
            UpdatePersonModel model,
            ClaimsPrincipal user,
            IPersonService personService,
            IValidator<UpdatePersonModel> validator,
            CancellationToken cancellationToken) =>
        {
            var requesterId = user.GetPersonId();

            if (requesterId == model.Id)
                return Results.Forbid();

            if (!user.GetIsAdmin() && !user.GetIsLeader())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await personService.UpdateAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        }).RequireAuthorization();

        group.MapPatch("/change-password", async (
            ChangePasswordModel model,
            ClaimsPrincipal user,
            IPersonService personService,
            IValidator<ChangePasswordModel> validator,
            CancellationToken cancellationToken) =>
        {
            var requesterId = user.GetPersonId();

            if (requesterId != model.PersonId)
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await personService.ChangePasswordAsync(model, cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization();

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
        }).AllowAnonymous();

        group.MapPatch("/reset-password", async (
            ResetPasswordModel model,
            ClaimsPrincipal user,
            IPersonService personService,
            IValidator<ResetPasswordModel> validator,
            CancellationToken cancellationToken) =>
        {
            var requesterId = user.GetPersonId();

            if (requesterId != model.PersonId)
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await personService.ResetPasswordAsync(model, cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization();

        group.MapPatch("/theme", async (
            UpdateThemeModel model,
            ClaimsPrincipal user,
            IPersonService personService,
            CancellationToken cancellationToken) =>
        {
            var personId = user.GetPersonId();
            await personService.UpdateThemeAsync(personId, model.IsBlackTheme, cancellationToken);
            return Results.NoContent();
        }).RequireAuthorization();

        return builder;
    }
}
