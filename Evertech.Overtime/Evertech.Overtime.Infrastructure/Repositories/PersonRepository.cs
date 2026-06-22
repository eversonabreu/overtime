using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;

namespace Evertech.Overtime.Infrastructure.Repositories;

internal sealed class PersonRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<Person>(unitOfWork), IPersonRepository
{
    protected override string TableName => "person";

    public override async Task<Guid> AddAsync(Person entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO person (id, name, registration, email, password, isActive, isPasswordPendingReset, isAdmin, hourlyRate, compensatoryTimeEnabled, municipalityId, createdAt, updatedAt)
            VALUES (@Id, @Name, @Registration, @Email, @Password, @IsActive, @IsPasswordPendingReset, @IsAdmin, @HourlyRate, @CompensatoryTimeEnabled, @MunicipalityId, @CreatedAt, @UpdatedAt)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<Person?> GetByRegistrationAsync(string registration, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM person WHERE registration = @Registration";
        var command = new CommandDefinition(sql, new { Registration = registration }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<Person>(command);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM person WHERE id = @Id)";
        var command = new CommandDefinition(sql, new { Id = id }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM person)";
        var command = new CommandDefinition(sql, transaction: UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.ExecuteScalarAsync<bool>(command);
    }

    public async Task<IReadOnlyList<Person>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM person WHERE isActive = TRUE ORDER BY name";
        var command = new CommandDefinition(sql, transaction: UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<Person>(command);
        return result.AsList();
    }

    public async Task UpdatePasswordAsync(Guid id, string encryptedPassword, bool isPasswordPendingReset, CancellationToken cancellationToken = default)
    {
        const string sql = """
            UPDATE person
            SET password = @Password,
                isPasswordPendingReset = @IsPasswordPendingReset,
                updatedAt = NOW()
            WHERE id = @Id
            """;

        var command = new CommandDefinition(sql, new { Id = id, Password = encryptedPassword, IsPasswordPendingReset = isPasswordPendingReset }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);
    }
}
