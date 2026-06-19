namespace Evertech.Overtime.Domain.Services.Abstractions;

public interface IHolidayService
{
    Task<bool> IsHolidayAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default);
    Task<string?> FindHolidayDescriptionAsync(DateOnly date, Guid municipalityId, CancellationToken cancellationToken = default);
}