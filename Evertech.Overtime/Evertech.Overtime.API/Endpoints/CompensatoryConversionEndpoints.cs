using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Application.Validators;
using FluentValidation;

namespace Evertech.Overtime.API.Endpoints;

public static class CompensatoryConversionEndpoints
{
    public static IEndpointRouteBuilder AddCompensatoryConversionEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/compensatory-conversions").WithTags("CompensatoryConversions").RequireAuthorization();

        group.MapPost("/", async (
            CreateCompensatoryConversionModel model,
            ICompensatoryConversionAppService conversionService,
            IValidator<CreateCompensatoryConversionModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var id = await conversionService.CreateAsync(model, cancellationToken);
            return Results.Ok(new { data = new { id } });
        });

        group.MapPut("/", async (
            UpdateCompensatoryConversionModel model,
            ICompensatoryConversionAppService conversionService,
            IValidator<UpdateCompensatoryConversionModel> validator,
            CancellationToken cancellationToken) =>
        {
            var errors = await ValidationHelper.ValidateAsync(validator, model, cancellationToken);
            if (errors is not null)
                return Results.BadRequest(new { errors });

            var result = await conversionService.UpdateAsync(model, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ICompensatoryConversionAppService conversionService,
            CancellationToken cancellationToken) =>
        {
            var result = await conversionService.DeleteAsync(id, cancellationToken);
            return result.IsSuccess
                ? Results.NoContent()
                : Results.UnprocessableEntity(new { message = result.Reason });
        });

        group.MapGet("/person/{personId:guid}/{year:int}/{month:int}", async (
            Guid personId,
            int year,
            int month,
            ICompensatoryConversionAppService conversionService,
            CancellationToken cancellationToken) =>
        {
            var result = await conversionService.GetByPersonAndMonthAsync(personId, year, month, cancellationToken);
            return Results.Ok(new { data = result });
        });

        group.MapGet("/balance/{personId:guid}", async (
            Guid personId,
            ICompensatoryConversionAppService conversionService,
            CancellationToken cancellationToken) =>
        {
            var balance = await conversionService.GetBalanceAsync(personId, cancellationToken);
            return Results.Ok(new { data = balance });
        });

        return builder;
    }
}
