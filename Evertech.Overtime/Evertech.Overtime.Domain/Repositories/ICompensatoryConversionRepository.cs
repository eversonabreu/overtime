using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Interfaces;

public interface ICompensatoryConversionRepository : IRepository<CompensatoryConversion>
{
    Task<int> GetAccumulatedMinutesAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<int> GetConvertedMinutesAsync(Guid personId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CompensatoryConversion>> GetByPersonAndMonthAsync(
        Guid personId,
        int year,
        int month,
        CancellationToken cancellationToken = default);
}