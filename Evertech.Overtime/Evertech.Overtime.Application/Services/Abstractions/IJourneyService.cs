using Evertech.Overtime.Application.Models;

namespace Evertech.Overtime.Application.Services.Abstractions;

public interface IJourneyService
{
    Task<Guid> CreateAsync(CreateJourneyModel model, CancellationToken cancellationToken = default);
    Task<JourneyResponseModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JourneyResponseModel>> GetByPersonAndMonthAsync(Guid personId, int year, int month, CancellationToken cancellationToken = default);
    Task<OperationResult> MarkAsInconsistentAsync(MarkJourneyInconsistentModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> DeleteAsync(Guid journeyId, CancellationToken cancellationToken = default);
}
