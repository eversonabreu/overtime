using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Interfaces;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class CompensatoryConversionRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<CompensatoryConversion>(unitOfWork), ICompensatoryConversionRepository
{
    protected override string TableName => "compensatory_conversion";

    public override async Task<Guid> AddAsync(CompensatoryConversion entity, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO compensatory_conversion (id, personId, conversionDate, minutes, type, createdAt)
            VALUES (@Id, @PersonId, @ConversionDate, @Minutes, @Type::conversion_type, @CreatedAt)
            """;

        var command = new CommandDefinition(sql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);

        return entity.Id;
    }

    public async Task<int> GetAccumulatedMinutesAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COALESCE(SUM(je.minutes), 0)
            FROM journey j
            JOIN journey_entry je ON je.journeyId = j.id
            WHERE j.personId = @PersonId
              AND j.inconsistent = FALSE
              AND je.type = 'COMPENSATORY'
            """;

        var command = new CommandDefinition(sql, new { PersonId = personId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        return await UnitOfWork.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<int> GetConvertedMinutesAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COALESCE(SUM(minutes), 0)
            FROM compensatory_conversion
            WHERE personId = @PersonId
            """;

        var command = new CommandDefinition(sql, new { PersonId = personId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        return await UnitOfWork.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<IReadOnlyList<CompensatoryConversion>> GetByPersonAndMonthAsync(
        Guid personId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT * FROM compensatory_conversion
            WHERE personId = @PersonId
              AND DATE_TRUNC('month', conversionDate) = DATE_TRUNC('month', MAKE_DATE(@Year, @Month, 1))
            ORDER BY conversionDate
            """;

        var command = new CommandDefinition(sql, new { PersonId = personId, Year = year, Month = month }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        var result = await UnitOfWork.Connection.QueryAsync<CompensatoryConversion>(command);

        return result.AsList();
    }
}