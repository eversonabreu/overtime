using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IGroupPersonRepository : IRepository<GroupPerson>
{
    Task<IReadOnlyList<GroupPerson>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPerson>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupPerson>> GetActiveByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<GroupPerson?> GetByGroupAndPersonAsync(Guid groupId, Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountLeadersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<bool> IsLeaderOfAnyGroupAsync(Guid personId, CancellationToken cancellationToken = default);
}
