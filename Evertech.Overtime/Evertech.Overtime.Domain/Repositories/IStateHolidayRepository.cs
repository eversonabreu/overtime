using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IStateHolidayRepository : IRepository<StateHoliday>
{
    Task<StateHoliday?> FindByMunicipalityAsync(int day, int month, Guid municipalityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StateHoliday>> GetByMonthAndMunicipalityAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default);
}