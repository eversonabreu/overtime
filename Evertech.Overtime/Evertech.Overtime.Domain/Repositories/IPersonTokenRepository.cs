using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IPersonTokenRepository : IRepository<PersonToken>
{
    Task<PersonToken?> GetActiveByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}