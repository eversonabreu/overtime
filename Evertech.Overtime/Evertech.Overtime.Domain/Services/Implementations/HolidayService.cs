using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Domain.Services.Implementations;

internal sealed class HolidayService(
    INationalHolidayRepository nationalHolidayRepository,
    IMunicipalityHolidayRepository municipalityHolidayRepository,
    IStateHolidayRepository stateHolidayRepository) : IHolidayService
{
    public async Task<bool> IsHolidayAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        var description = await FindHolidayDescriptionAsync(date, municipalityId, cancellationToken);
        return description is not null;
    }

    public async Task<string?> FindHolidayDescriptionAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        var national = await nationalHolidayRepository.FindAsync(date.Day, date.Month, cancellationToken);
        if (national is not null)
            return national.Description;

        var municipality = await municipalityHolidayRepository.FindAsync(date.Day, date.Month, municipalityId, cancellationToken);
        if (municipality is not null)
            return municipality.Description;

        var state = await stateHolidayRepository.FindByMunicipalityAsync(date.Day, date.Month, municipalityId, cancellationToken);
        return state?.Description;
    }
}