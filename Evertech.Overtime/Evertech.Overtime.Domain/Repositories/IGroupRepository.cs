using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface IGroupRepository : IRepository<Group>
{
    Task<IReadOnlyList<Group>> GetActiveAsync(CancellationToken cancellationToken = default);
}