using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class MunicipalityHolidayRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<MunicipalityHoliday>(unitOfWork), IMunicipalityHolidayRepository
{
    protected override string TableName => "municipality_holiday";

    public override async Task<Guid> AddAsync(MunicipalityHoliday entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO municipality_holiday (id, day, month, description, municipalityId)
            VALUES (@Id, @Day, @Month, @Description, @MunicipalityId)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<MunicipalityHoliday?> FindAsync(int day, int month, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM municipality_holiday WHERE day = @Day AND month = @Month AND municipalityId = @MunicipalityId";
        var command = new CommandDefinition(sql, new { Day = day, Month = month, MunicipalityId = municipalityId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<MunicipalityHoliday>(command);
    }

    public async Task<IReadOnlyList<MunicipalityHoliday>> GetByMonthAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM municipality_holiday WHERE month = @Month AND municipalityId = @MunicipalityId ORDER BY day";
        var command = new CommandDefinition(sql, new { Month = month, MunicipalityId = municipalityId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<MunicipalityHoliday>(command);
        return result.AsList();
    }
}