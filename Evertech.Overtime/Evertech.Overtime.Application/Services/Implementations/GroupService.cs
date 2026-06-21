using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Domain.Exceptions;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using GroupEntity = Evertech.Overtime.Domain.Entities.Group;
using GroupPersonEntity = Evertech.Overtime.Domain.Entities.GroupPerson;

namespace Evertech.Overtime.Application.Services.Implementations;

internal sealed class GroupService(
    IGroupRepository groupRepository,
    IGroupPersonRepository groupPersonRepository,
    IPersonRepository personRepository,
    IDbUnitOfWork unitOfWork) : IGroupService
{
    public async Task<Guid> CreateAsync(CreateGroupModel model, CancellationToken cancellationToken = default)
    {
        var leader = await personRepository.GetByIdAsync(model.LeaderPersonId, cancellationToken)
            ?? throw new BusinessException("A pessoa indicada como líder não foi encontrada.");

        if (leader.IsAdmin)
            throw new BusinessException("Um administrador não pode ser vinculado a um grupo.");

        var group = new GroupEntity
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Observation = model.Observation,
            IsActive = true
        };

        var leaderLink = new GroupPersonEntity
        {
            Id = Guid.NewGuid(),
            GroupId = group.Id,
            PersonId = leader.Id,
            IsLeader = true
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var id = await groupRepository.AddAsync(group, cancellationToken);
            await groupPersonRepository.AddAsync(leaderLink, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<GroupResponseModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetByIdAsync(id, cancellationToken);
        return group is null ? null : MapToResponse(group);
    }

    public async Task<IReadOnlyList<GroupResponseModel>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        var groups = await groupRepository.GetActiveAsync(cancellationToken);
        return groups.Select(MapToResponse).ToList();
    }

    public async Task UpdateAsync(UpdateGroupModel model, CancellationToken cancellationToken = default)
    {
        var current = await groupRepository.GetByIdAsync(model.Id, cancellationToken);
        if (current is null)
            return;

        var group = new GroupEntity
        {
            Id = current.Id,
            Name = model.Name,
            Observation = model.Observation,
            IsActive = model.IsActive
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await groupRepository.UpdateAsync(group, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OperationResult> AddMemberAsync(AddPersonToGroupModel model, CancellationToken cancellationToken = default)
    {
        var group = await groupRepository.GetByIdAsync(model.GroupId, cancellationToken);
        if (group is null)
            return OperationResult.Failure("O grupo informado não foi encontrado.");

        if (!group.IsActive)
            return OperationResult.Failure("Não é possível adicionar membros a um grupo desativado.");

        var person = await personRepository.GetByIdAsync(model.PersonId, cancellationToken);
        if (person is null)
            return OperationResult.Failure("A pessoa informada não foi encontrada.");

        if (person.IsAdmin)
            return OperationResult.Failure("Um administrador não pode ser adicionado a um grupo.");

        var existingLink = await groupPersonRepository.GetByGroupAndPersonAsync(model.GroupId, model.PersonId, cancellationToken);
        if (existingLink is not null)
            return OperationResult.Failure("Esta pessoa já pertence ao grupo.");

        var link = new GroupPersonEntity
        {
            Id = Guid.NewGuid(),
            GroupId = model.GroupId,
            PersonId = model.PersonId,
            IsLeader = false
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await groupPersonRepository.AddAsync(link, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OperationResult> RemoveMemberAsync(RemoveGroupMemberModel model, CancellationToken cancellationToken = default)
    {
        var link = await groupPersonRepository.GetByGroupAndPersonAsync(model.GroupId, model.PersonId, cancellationToken);
        if (link is null)
            return OperationResult.Failure("Esta pessoa não pertence ao grupo informado.");

        if (link.IsLeader)
        {
            var leaderCount = await groupPersonRepository.CountLeadersByGroupIdAsync(model.GroupId, cancellationToken);
            if (leaderCount <= 1)
                return OperationResult.Failure("Não é possível remover esta pessoa pois ela é a única líder do grupo. Promova outro líder antes de removê-la.");
        }

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await groupPersonRepository.DeleteAsync(link.Id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OperationResult> SetLeaderAsync(SetGroupLeaderModel model, CancellationToken cancellationToken = default)
    {
        var link = await groupPersonRepository.GetByGroupAndPersonAsync(model.GroupId, model.PersonId, cancellationToken);
        if (link is null)
            return OperationResult.Failure("Esta pessoa não pertence ao grupo informado.");

        if (link.IsLeader == model.IsLeader)
            return OperationResult.Success();

        if (!model.IsLeader)
        {
            var leaderCount = await groupPersonRepository.CountLeadersByGroupIdAsync(model.GroupId, cancellationToken);
            if (leaderCount <= 1)
                return OperationResult.Failure("Não é possível remover a liderança desta pessoa pois um grupo não pode ficar sem nenhum líder.");
        }

        var updatedLink = new GroupPersonEntity
        {
            Id = link.Id,
            GroupId = link.GroupId,
            PersonId = link.PersonId,
            IsLeader = model.IsLeader
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await groupPersonRepository.UpdateAsync(updatedLink, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static GroupResponseModel MapToResponse(GroupEntity group) => new()
    {
        Id = group.Id,
        Name = group.Name,
        Observation = group.Observation,
        IsActive = group.IsActive
    };
}