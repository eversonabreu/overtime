using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IHolidayRepository : IRepository<Holiday>
{
    Task<Holiday?> FindHolidayAsync(
        int day,
        int month,
        Guid municipalityId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Holiday>> GetByMonthAndMunicipalityAsync(
        int month,
        Guid municipalityId,
        CancellationToken cancellationToken = default);
}