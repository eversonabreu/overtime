using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;

namespace Evertech.Overtime.Infrastructure.Repositories;

internal sealed class CountryRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<Country>(unitOfWork), ICountryRepository
{
    protected override string TableName => "country";

    public override async Task<Guid> AddAsync(Country entity, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO country (id, name) VALUES (@Id, @Name)";
        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);
        return entity.Id;
    }

    public async Task<IReadOnlyList<Country>> GetAllOrderedAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM country ORDER BY name";
        var command = new CommandDefinition(sql, transaction: UnitOfWork.Transaction, cancellationToken: cancellationToken);
        var result = await UnitOfWork.Connection.QueryAsync<Country>(command);
        return result.AsList();
    }
}

internal sealed class StateRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<State>(unitOfWork), IStateRepository
{
    protected override string TableName => "state";

    public override async Task<Guid> AddAsync(State entity, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO state (id, name, code, countryId) VALUES (@Id, @Name, @Code, @CountryId)";
        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);
        return entity.Id;
    }

    public async Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM state WHERE countryId = @CountryId ORDER BY name";
        var command = new CommandDefinition(sql, new { CountryId = countryId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        var result = await UnitOfWork.Connection.QueryAsync<State>(command);
        return result.AsList();
    }
}

internal sealed class MunicipalityRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<Municipality>(unitOfWork), IMunicipalityRepository
{
    protected override string TableName => "municipality";

    public override async Task<Guid> AddAsync(Municipality entity, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO municipality (id, name, stateId) VALUES (@Id, @Name, @StateId)";
        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);
        return entity.Id;
    }

    public async Task<IReadOnlyList<Municipality>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM municipality WHERE stateId = @StateId ORDER BY name";
        var command = new CommandDefinition(sql, new { StateId = stateId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        var result = await UnitOfWork.Connection.QueryAsync<Municipality>(command);
        return result.AsList();
    }
}
