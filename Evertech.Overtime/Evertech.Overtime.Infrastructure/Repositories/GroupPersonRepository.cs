using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class GroupPersonRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<GroupPerson>(unitOfWork), IGroupPersonRepository
{
    protected override string TableName => "group_persons";

    public override async Task<Guid> AddAsync(GroupPerson entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO group_persons (id, groupId, personId, isLeader)
            VALUES (@Id, @GroupId, @PersonId, @IsLeader)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<IReadOnlyList<GroupPerson>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM group_persons WHERE groupId = @GroupId";
        var command = new CommandDefinition(sql, new { GroupId = groupId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<GroupPerson>(command);
        return result.AsList();
    }

    public async Task<IReadOnlyList<GroupPerson>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM group_persons WHERE personId = @PersonId";
        var command = new CommandDefinition(sql, new { PersonId = personId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<GroupPerson>(command);
        return result.AsList();
    }

    public async Task<GroupPerson> GetByGroupAndPersonAsync(Guid groupId, Guid personId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM group_persons WHERE groupId = @GroupId AND personId = @PersonId";
        var command = new CommandDefinition(sql, new { GroupId = groupId, PersonId = personId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<GroupPerson>(command);
    }

    public async Task<int> CountLeadersByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT COUNT(*) FROM group_persons WHERE groupId = @GroupId AND isLeader = TRUE";
        var command = new CommandDefinition(sql, new { GroupId = groupId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<bool> IsLeaderOfAnyGroupAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM group_persons WHERE personId = @PersonId AND isLeader = TRUE)";
        var command = new CommandDefinition(sql, new { PersonId = personId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.ExecuteScalarAsync<bool>(command);
    }
}