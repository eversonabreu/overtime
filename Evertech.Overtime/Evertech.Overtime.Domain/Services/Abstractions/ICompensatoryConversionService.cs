namespace Evertech.Overtime.Domain.Services.Abstractions;

public interface ICompensatoryConversionService
{
    Task<bool> CanConvertAsync(Guid personId, int minutes, CancellationToken cancellationToken = default);

    Task<bool> CanEditAsync(
        Guid personId,
        int currentMinutes,
        int newMinutes,
        CancellationToken cancellationToken = default);

    decimal CalculateRemuneratedAmount(decimal hourlyRate, int minutes);
}