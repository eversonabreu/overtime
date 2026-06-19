namespace Evertech.Overtime.Domain.Services.Abstractions;

public interface ICompensatoryBalanceService
{
    Task<int> GetAvailableBalanceAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<bool> HasSufficientBalanceAsync(Guid personId, int minutes, CancellationToken cancellationToken = default);
}