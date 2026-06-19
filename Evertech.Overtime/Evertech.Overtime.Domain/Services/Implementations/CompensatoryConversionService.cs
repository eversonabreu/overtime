using Evertech.Overtime.Domain.Helpers;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Domain.Services.Implementations;

internal sealed class CompensatoryConversionService(ICompensatoryBalanceService compensatoryBalanceService) : ICompensatoryConversionService
{
    private const decimal RemuneratedConversionRate = 1.50m;

    public Task<bool> CanConvertAsync(Guid personId, int minutes, CancellationToken cancellationToken = default) =>
        compensatoryBalanceService.HasSufficientBalanceAsync(personId, minutes, cancellationToken);

    public async Task<bool> CanEditAsync(
        Guid personId,
        int currentMinutes,
        int newMinutes,
        CancellationToken cancellationToken = default)
    {
        if (newMinutes <= currentMinutes)
            return true;

        var difference = newMinutes - currentMinutes;
        return await compensatoryBalanceService.HasSufficientBalanceAsync(personId, difference, cancellationToken);
    }

    public decimal CalculateRemuneratedAmount(decimal hourlyRate, int minutes)
    {
        var value = (hourlyRate / 60m) * RemuneratedConversionRate * minutes;
        return RoundingHelper.RoundMonetary(value);
    }
}