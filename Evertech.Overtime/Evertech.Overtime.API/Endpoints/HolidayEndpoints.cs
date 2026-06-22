using System.Security.Claims;
using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using Evertech.Overtime.API.Extensions;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class HolidayEndpoints
{
    public static IEndpointRouteBuilder AddHolidayEndpoints(this IEndpointRouteBuilder builder)
    {
        AddNationalHolidayEndpoints(builder);
        AddStateHolidayEndpoints(builder);
        AddMunicipalityHolidayEndpoints(builder);

        return builder;
    }

    private static void AddNationalHolidayEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/holidays/national").WithTags("Holidays").RequireAuthorization();

        group.MapPost("/", async (
            CreateNationalHolidayModel model,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            IValidator<CreateNationalHolidayModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await holidayService.CreateNationalAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapPut("/", async (
            UpdateNationalHolidayModel model,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            IValidator<UpdateNationalHolidayModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await holidayService.UpdateNationalAsync(model, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            await holidayService.DeleteNationalAsync(id, cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/{month:int}", async (
            int month,
            IHolidayAppService holidayService,
            CancellationToken cancellationToken) =>
        {
            var result = await holidayService.GetNationalByMonthAsync(month, cancellationToken);
            return Results.Ok(new { data = result });
        });
    }

    private static void AddStateHolidayEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/holidays/state").WithTags("Holidays");

        group.MapPost("/", async (
            CreateStateHolidayModel model,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            IValidator<CreateStateHolidayModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await holidayService.CreateStateAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapPut("/", async (
            UpdateStateHolidayModel model,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            IValidator<UpdateStateHolidayModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await holidayService.UpdateStateAsync(model, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            await holidayService.DeleteStateAsync(id, cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/{month:int}/{municipalityId:guid}", async (
            int month,
            Guid municipalityId,
            IHolidayAppService holidayService,
            CancellationToken cancellationToken) =>
        {
            var result = await holidayService.GetStateByMonthAndMunicipalityAsync(month, municipalityId, cancellationToken);
            return Results.Ok(new { data = result });
        });
    }

    private static void AddMunicipalityHolidayEndpoints(IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/holidays/municipality").WithTags("Holidays");

        group.MapPost("/", async (
            CreateMunicipalityHolidayModel model,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            IValidator<CreateMunicipalityHolidayModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await holidayService.CreateMunicipalityAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapPut("/", async (
            UpdateMunicipalityHolidayModel model,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            IValidator<UpdateMunicipalityHolidayModel> validator,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            await holidayService.UpdateMunicipalityAsync(model, cancellationToken);
            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ClaimsPrincipal user,
            IHolidayAppService holidayService,
            CancellationToken cancellationToken) =>
        {
            if (!user.GetIsAdmin())
                return Results.Forbid();

            await holidayService.DeleteMunicipalityAsync(id, cancellationToken);
            return Results.NoContent();
        });

        group.MapGet("/{month:int}/{municipalityId:guid}", async (
            int month,
            Guid municipalityId,
            IHolidayAppService holidayService,
            CancellationToken cancellationToken) =>
        {
            var result = await holidayService.GetMunicipalityByMonthAsync(month, municipalityId, cancellationToken);
            return Results.Ok(new { data = result });
        });
    }
}