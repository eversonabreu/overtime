using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface INationalHolidayRepository : IRepository<NationalHoliday>
{
    Task<NationalHoliday?> FindAsync(int day, int month, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NationalHoliday>> GetByMonthAsync(int month, CancellationToken cancellationToken = default);
}