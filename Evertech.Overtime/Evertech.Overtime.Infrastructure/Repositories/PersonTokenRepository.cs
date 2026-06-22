using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class PersonTokenRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<PersonToken>(unitOfWork), IPersonTokenRepository
{
    protected override string TableName => "person_tokens";

    public override async Task<Guid> AddAsync(PersonToken entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO person_tokens (id, personId, refreshToken, expiresAt, isRevoked, createdAt)
            VALUES (@Id, @PersonId, @RefreshToken, @ExpiresAt, @IsRevoked, @CreatedAt)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<PersonToken?> GetActiveByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT * FROM person_tokens
            WHERE refreshToken = @RefreshToken
              AND isRevoked = FALSE
              AND expiresAt > NOW()
            """;

        var command = new CommandDefinition(sql, new { RefreshToken = refreshToken }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<PersonToken>(command);
    }

    public async Task RevokeAllByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE person_tokens
            SET isRevoked = TRUE
            WHERE personId = @PersonId AND isRevoked = FALSE
            """;

        var command = new CommandDefinition(sql, new { PersonId = personId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);
    }
}