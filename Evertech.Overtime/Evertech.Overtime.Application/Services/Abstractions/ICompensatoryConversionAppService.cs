using Evertech.Overtime.Application.Models;

namespace Evertech.Overtime.Application.Services.Abstractions;

public interface ICompensatoryConversionAppService
{
    Task<Guid> CreateAsync(CreateCompensatoryConversionModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> UpdateAsync(UpdateCompensatoryConversionModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CompensatoryConversionResponseModel>> GetByPersonAndMonthAsync(Guid personId, int year, int month, CancellationToken cancellationToken = default);
    Task<CompensatoryBalanceResponseModel> GetBalanceAsync(Guid personId, CancellationToken cancellationToken = default);
}
