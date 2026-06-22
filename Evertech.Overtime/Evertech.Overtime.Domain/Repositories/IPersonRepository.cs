using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByRegistrationAsync(string registration, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Person>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(Guid id, string encryptedPassword, bool isPasswordPendingReset, CancellationToken cancellationToken = default);
}

