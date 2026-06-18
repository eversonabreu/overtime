using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IJourneyRepository : IRepository<Journey>
{
    Task<IReadOnlyList<Journey>> GetByPersonAndMonthAsync(
        Guid personId,
        int year,
        int month,
        CancellationToken cancellationToken = default);

    Task<int> GetTotalMinutesByPersonAndDayAsync(
        Guid personId,
        DateOnly day,
        CancellationToken cancellationToken = default);

    Task<Journey?> GetWithEntriesAsync(Guid journeyId, CancellationToken cancellationToken = default);
}