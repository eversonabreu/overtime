using Evertech.Overtime.Domain.Entities;

namespace Evertech.Overtime.Domain.Services.Abstractions;

public interface IJourneyDecomposerService
{
    Task<IReadOnlyList<JourneyEntry>> DecomposeAsync(
        Guid journeyId,
        DateTimeOffset checkIn,
        DateTimeOffset checkOut,
        decimal hourlyRate,
        bool compensatoryTimeEnabled,
        Guid municipalityId,
        CancellationToken cancellationToken = default);
}