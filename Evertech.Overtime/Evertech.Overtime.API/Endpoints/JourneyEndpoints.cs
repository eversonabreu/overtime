using System.Security.Claims;
using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using Evertech.Overtime.API.Extensions;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class JourneyEndpoints
{
    public static IEndpointRouteBuilder AddJourneyEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/journeys").WithTags("Journeys").RequireAuthorization();

        group.MapPost("/", async (
            CreateJourneyModel model,
            ClaimsPrincipal user,
            IJourneyService journeyService,
            IValidator<CreateJourneyModel> validator,
            CancellationToken cancellationToken) =>
        {
            var requesterId = user.GetPersonId();

            if (requesterId != model.PersonId && !user.GetIsAdmin() && !user.GetIsLeader())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await journeyService.CreateAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapGet("/{id:guid}", async (
            Guid id,
            IJourneyService journeyService,
            CancellationToken cancellationToken) =>
        {
            var journey = await journeyService.GetByIdAsync(id, cancellationToken);
            return journey is null
                ? Results.NotFound()
                : Results.Ok(new { data = journey });
        });

        group.MapGet("/person/{personId:guid}/{year:int}/{month:int}", async (
            Guid personId,
            int year,
            int month,
            IJourneyService journeyService,
            CancellationToken cancellationToken) =>
        {
            var journeys = await journeyService.GetByPersonAndMonthAsync(personId, year, month, cancellationToken);
            return Results.Ok(new { data = journeys });
        });

        group.MapPatch("/inconsistent", async (
            MarkJourneyInconsistentModel model,
            ClaimsPrincipal user,
            IJourneyService journeyService,
            IValidator<MarkJourneyInconsistentModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin() && !user.GetIsLeader())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await journeyService.MarkAsInconsistentAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IJourneyService journeyService,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin() && !user.GetIsLeader())
                return Results.Forbid();

            var result = await journeyService.DeleteAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        return builder;
    }
}