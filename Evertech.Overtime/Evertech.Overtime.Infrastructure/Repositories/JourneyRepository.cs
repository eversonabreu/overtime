using Dapper;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;
using Evertech.Overtime.Infrastructure.Repositories.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Infrastructure.Repositories;

[ExcludeFromCodeCoverage]
internal sealed class JourneyRepository(IDbUnitOfWork unitOfWork) : RepositoryBase<Journey>(unitOfWork), IJourneyRepository
{
    protected override string TableName => "journey";

    public override async Task<Guid> AddAsync(Journey entity, CancellationToken cancellationToken = default)
    {
        const string journeySql = """
            INSERT INTO journey (id, personId, checkIn, checkOut, totalMinutes, inconsistent, inconsistencyReason, createdAt)
            VALUES (@Id, @PersonId, @CheckIn, @CheckOut, @TotalMinutes, @Inconsistent, @InconsistencyReason, @CreatedAt)
            """;

        var journeyCommand = new CommandDefinition(journeySql, entity, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(journeyCommand);

        if (entity.Entries.Count > 0)
            await InsertEntriesAsync(entity.Entries, cancellationToken);

        return entity.Id;
    }

    public async Task<IReadOnlyList<Journey>> GetByPersonAndMonthAsync(
        Guid personId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT * FROM journey
            WHERE personId = @PersonId
              AND DATE_TRUNC('month', checkIn) = DATE_TRUNC('month', MAKE_DATE(@Year, @Month, 1))
            ORDER BY checkIn
            """;

        var command = new CommandDefinition(sql, new { PersonId = personId, Year = year, Month = month }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        var journeys = await UnitOfWork.Connection.QueryAsync<Journey>(command);

        var result = new List<Journey>();
        foreach (var journey in journeys)
        {
            var entries = await GetEntriesByJourneyIdAsync(journey.Id, cancellationToken);
            result.Add(AttachEntries(journey, entries));
        }

        return result;
    }

    public async Task<int> GetTotalMinutesByPersonAndDayAsync(
        Guid personId,
        DateOnly day,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT COALESCE(SUM(je.minutes), 0)
            FROM journey j
            JOIN journey_entry je ON je.journeyId = j.id
            WHERE j.personId = @PersonId
              AND j.inconsistent = FALSE
              AND DATE(j.checkIn) = @Day
            """;

        var command = new CommandDefinition(sql, new { PersonId = personId, Day = day.ToDateTime(TimeOnly.MinValue) }, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        return await UnitOfWork.Connection.ExecuteScalarAsync<int>(command);
    }

    public async Task<Journey?> GetWithEntriesAsync(Guid journeyId, CancellationToken cancellationToken = default)
    {
        var journey = await GetByIdAsync(journeyId, cancellationToken);
        if (journey is null)
            return null;

        var entries = await GetEntriesByJourneyIdAsync(journeyId, cancellationToken);
        return AttachEntries(journey, entries);
    }

    private async Task InsertEntriesAsync(IReadOnlyList<JourneyEntry> entries, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO journey_entry (id, journeyId, checkIn, checkOut, minutes, baseRate, grossAmount, type, createdAt)
            VALUES (@Id, @JourneyId, @CheckIn, @CheckOut, @Minutes, @BaseRate, @GrossAmount, @Type::entry_type, @CreatedAt)
            """;

        var command = new CommandDefinition(sql, entries, UnitOfWork.Transaction, cancellationToken: cancellationToken);
        await UnitOfWork.Connection.ExecuteAsync(command);
    }

    private async Task<IReadOnlyList<JourneyEntry>> GetEntriesByJourneyIdAsync(Guid journeyId, CancellationToken cancellationToken)
    {
        const string sql = "SELECT * FROM journey_entry WHERE journeyId = @JourneyId ORDER BY checkIn";
        var command = new CommandDefinition(sql, new { JourneyId = journeyId }, UnitOfWork.Transaction, cancellationToken: cancellationToken);

        var result = await UnitOfWork.Connection.QueryAsync<JourneyEntry>(command);
        return result.AsList();
    }

    private static Journey AttachEntries(Journey journey, IReadOnlyList<JourneyEntry> entries) => new()
    {
        Id = journey.Id,
        PersonId = journey.PersonId,
        CheckIn = journey.CheckIn,
        CheckOut = journey.CheckOut,
        TotalMinutes = journey.TotalMinutes,
        Inconsistent = journey.Inconsistent,
        InconsistencyReason = journey.InconsistencyReason,
        CreatedAt = journey.CreatedAt,
        Entries = entries
    };
}