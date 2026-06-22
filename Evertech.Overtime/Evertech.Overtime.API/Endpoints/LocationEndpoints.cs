using Evertech.Overtime.Domain.Repositories;

namespace Evertech.Overtime.API.Endpoints;

public static class LocationEndpoints
{
    public static IEndpointRouteBuilder AddLocationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/locations").WithTags("Locations").RequireAuthorization();

        group.MapGet("/countries", async (
            ICountryRepository countryRepository,
            CancellationToken cancellationToken) =>
        {
            var countries = await countryRepository.GetAllOrderedAsync(cancellationToken);
            return Results.Ok(new { data = countries.Select(c => new { c.Id, c.Name }) });
        });

        group.MapGet("/states/{countryId:guid}", async (
            Guid countryId,
            IStateRepository stateRepository,
            CancellationToken cancellationToken) =>
        {
            var states = await stateRepository.GetByCountryIdAsync(countryId, cancellationToken);
            return Results.Ok(new { data = states.Select(s => new { s.Id, s.Name, s.Code }) });
        });

        group.MapGet("/municipalities/{stateId:guid}", async (
            Guid stateId,
            IMunicipalityRepository municipalityRepository,
            CancellationToken cancellationToken) =>
        {
            var municipalities = await municipalityRepository.GetByStateIdAsync(stateId, cancellationToken);
            return Results.Ok(new { data = municipalities.Select(m => new { m.Id, m.Name }) });
        });

        return builder;
    }
}
