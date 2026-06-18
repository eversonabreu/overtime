using Evertech.Overtime.Domain.Entities;

namespace Evertech.Overtime.Domain.Services.Abstractions;

public interface IHolidayCheckerService
{
    Task<bool> IsHolidayAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default);
    Task<Holiday?> FindHolidayAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default);
}