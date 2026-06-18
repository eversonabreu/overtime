using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Domain.Services.Implementations;

internal sealed class HolidayCheckerService(IHolidayRepository holidayRepository) : IHolidayCheckerService
{
    public async Task<bool> IsHolidayAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        var holiday = await FindHolidayAsync(date, municipalityId, cancellationToken);
        return holiday is not null;
    }

    public Task<Holiday?> FindHolidayAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default) =>
        holidayRepository.FindHolidayAsync(date.Day, date.Month, municipalityId, cancellationToken);
}