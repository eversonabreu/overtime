using Evertech.Overtime.Application.Models;

namespace Evertech.Overtime.Application.Services.Abstractions;

public interface IGroupService
{
    Task<Guid> CreateAsync(CreateGroupModel model, CancellationToken cancellationToken = default);
    Task<GroupResponseModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupResponseModel>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateGroupModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> AddMemberAsync(AddPersonToGroupModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> RemoveMemberAsync(RemoveGroupMemberModel model, CancellationToken cancellationToken = default);
    Task<OperationResult> SetLeaderAsync(SetGroupLeaderModel model, CancellationToken cancellationToken = default);
}