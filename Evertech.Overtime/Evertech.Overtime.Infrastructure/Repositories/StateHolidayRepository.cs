using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class StateHolidayRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<StateHoliday>(unitOfWork), IStateHolidayRepository
{
    protected override string TableName => "state_holiday";

    public override async Task<Guid> AddAsync(StateHoliday entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO state_holiday (id, day, month, description, stateId)
            VALUES (@Id, @Day, @Month, @Description, @StateId)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<StateHoliday?> FindByMunicipalityAsync(int day, int month, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT sh.*
            FROM state_holiday sh
            JOIN municipality m ON m.stateId = sh.stateId
            WHERE sh.day = @Day AND sh.month = @Month AND m.id = @MunicipalityId
            """;

        var command = new CommandDefinition(sql, new { Day = day, Month = month, MunicipalityId = municipalityId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<StateHoliday>(command);
    }

    public async Task<IReadOnlyList<StateHoliday>> GetByMonthAndMunicipalityAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT sh.*
            FROM state_holiday sh
            JOIN municipality m ON m.stateId = sh.stateId
            WHERE sh.month = @Month AND m.id = @MunicipalityId
            ORDER BY sh.day
            """;

        var command = new CommandDefinition(sql, new { Month = month, MunicipalityId = municipalityId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<StateHoliday>(command);
        return result.AsList();
    }
}