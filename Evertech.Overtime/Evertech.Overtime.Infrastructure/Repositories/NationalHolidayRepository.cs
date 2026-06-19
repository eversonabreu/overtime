using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class NationalHolidayRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<NationalHoliday>(unitOfWork), INationalHolidayRepository
{
    protected override string TableName => "national_holiday";

    public override async Task<Guid> AddAsync(NationalHoliday entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO national_holiday (id, day, month, description)
            VALUES (@Id, @Day, @Month, @Description)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<NationalHoliday?> FindAsync(int day, int month, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM national_holiday WHERE day = @Day AND month = @Month";
        var command = new CommandDefinition(sql, new { Day = day, Month = month }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        return await UnitOfWork.Connection.QuerySingleOrDefaultAsync<NationalHoliday>(command);
    }

    public async Task<IReadOnlyList<NationalHoliday>> GetByMonthAsync(int month, CancellationToken cancellationToken = default)
    {
        const string sql = "SELECT * FROM national_holiday WHERE month = @Month ORDER BY day";
        var command = new CommandDefinition(sql, new { Month = month }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<NationalHoliday>(command);
        return result.AsList();
    }
}