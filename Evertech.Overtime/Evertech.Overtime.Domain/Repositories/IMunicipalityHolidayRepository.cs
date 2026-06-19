using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IMunicipalityHolidayRepository : IRepository<MunicipalityHoliday>
{
    Task<MunicipalityHoliday?> FindAsync(int day, int month, Guid municipalityId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MunicipalityHoliday>> GetByMonthAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default);
}