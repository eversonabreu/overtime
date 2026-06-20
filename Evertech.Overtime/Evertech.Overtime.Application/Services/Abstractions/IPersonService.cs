using Evertech.Overtime.Application.Models;

namespace Evertech.Overtime.Application.Services.Abstractions;

public interface IPersonService
{
    Task<Guid?> CreateFirstAsync(CreateFirstPersonModel model, CancellationToken cancellationToken = default);
    Task<Guid> CreateAsync(CreatePersonModel model, CancellationToken cancellationToken = default);
    Task<PersonResponseModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdatePersonModel model, CancellationToken cancellationToken = default);
    Task ChangePasswordAsync(ChangePasswordModel model, CancellationToken cancellationToken = default);
    Task RequestPasswordResetAsync(RequestPasswordResetModel model, CancellationToken cancellationToken = default);
    Task ResetPasswordAsync(ResetPasswordModel model, CancellationToken cancellationToken = default);
}