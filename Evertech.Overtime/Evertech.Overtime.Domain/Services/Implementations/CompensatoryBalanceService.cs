using Evertech.Overtime.Domain.Interfaces;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Domain.Services.Implementations;

internal sealed class CompensatoryBalanceService(ICompensatoryConversionRepository compensatoryConversionRepository) : ICompensatoryBalanceService
{
    public async Task<int> GetAvailableBalanceAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var accumulated = await compensatoryConversionRepository.GetAccumulatedMinutesAsync(personId, cancellationToken);
        var converted = await compensatoryConversionRepository.GetConvertedMinutesAsync(personId, cancellationToken);

        return accumulated - converted;
    }

    public async Task<bool> HasSufficientBalanceAsync(Guid personId, int minutes, CancellationToken cancellationToken = default)
    {
        var balance = await GetAvailableBalanceAsync(personId, cancellationToken);
        return minutes <= balance;
    }
}