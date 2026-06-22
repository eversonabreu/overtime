using Evertech.Overtime.Application.Models;

namespace Evertech.Overtime.Application.Services.Abstractions;

public interface IAuthService
{
    Task<TokenResponseModel> LoginAsync(LoginModel model, CancellationToken cancellationToken = default);
    Task<TokenResponseModel> RefreshAsync(RefreshTokenModel model, CancellationToken cancellationToken = default);
    Task RevokeAsync(RevokeTokenModel model, CancellationToken cancellationToken = default);
}
