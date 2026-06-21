using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class GroupRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<Group>(unitOfWork), IGroupRepository
{
    protected override string TableName => "groups";

    public override async Task<Guid> AddAsync(Group entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO groups (id, name, observation, isActive)
            VALUES (@Id, @Name, @Observation, @IsActive)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<IReadOnlyList<Group>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM groups WHERE isActive = TRUE ORDER BY name";
        var command = new CommandDefinition(sql, transaction: UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<Group>(command);
        return result.AsList();
    }
}