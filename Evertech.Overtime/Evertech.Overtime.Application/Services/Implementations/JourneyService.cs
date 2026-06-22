using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Exceptions;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Application.Services.Implementations;

internal sealed class JourneyService(
    IJourneyRepository journeyRepository,
    IPersonRepository personRepository,
    IJourneyDecomposerService journeyDecomposerService,
    IDbUnitOfWork unitOfWork) : IJourneyService
{
    private const int MaxDailyMinutes = 480;

    public async Task<Guid> CreateAsync(CreateJourneyModel model, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(model.PersonId, cancellationToken)
            ?? throw new BusinessException("Colaborador não encontrado.");

        var totalMinutes = (int)(model.CheckOut - model.CheckIn).TotalMinutes;

        var dayTotal = await journeyRepository.GetTotalMinutesByPersonAndDayAsync(
            model.PersonId,
            DateOnly.FromDateTime(model.CheckIn.DateTime),
            cancellationToken);

        if (dayTotal + totalMinutes > MaxDailyMinutes)
            throw new BusinessException(
                $"O lançamento ultrapassa o limite diário de {MaxDailyMinutes} minutos. " +
                $"Minutos já lançados hoje: {dayTotal}. Disponível: {MaxDailyMinutes - dayTotal}.");

        var entries = await journeyDecomposerService.DecomposeAsync(
            journeyId: Guid.NewGuid(),
            checkIn: model.CheckIn,
            checkOut: model.CheckOut,
            hourlyRate: person.HourlyRate,
            compensatoryTimeEnabled: person.CompensatoryTimeEnabled,
            municipalityId: person.MunicipalityId,
            cancellationToken: cancellationToken);

        var journey = new Journey
        {
            Id = entries.First().JourneyId,
            PersonId = model.PersonId,
            CheckIn = model.CheckIn,
            CheckOut = model.CheckOut,
            TotalMinutes = totalMinutes,
            Inconsistent = false,
            CreatedAt = DateTimeOffset.UtcNow,
            Entries = entries
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await journeyRepository.AddAsync(journey, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return journey.Id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<JourneyResponseModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var journey = await journeyRepository.GetWithEntriesAsync(id, cancellationToken);
        return journey is null ? null : MapToResponse(journey);
    }

    public async Task<IReadOnlyList<JourneyResponseModel>> GetByPersonAndMonthAsync(
        Guid personId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var journeys = await journeyRepository.GetByPersonAndMonthAsync(personId, year, month, cancellationToken);
        return journeys.Select(MapToResponse).ToList();
    }

    public async Task<OperationResult> MarkAsInconsistentAsync(
        MarkJourneyInconsistentModel model,
        CancellationToken cancellationToken = default)
    {
        var journey = await journeyRepository.GetByIdAsync(model.JourneyId, cancellationToken);
        if (journey is null)
            return OperationResult.Failure("Jornada não encontrada.");

        if (!IsCurrentMonth(journey.CheckIn))
            return OperationResult.Failure("Não é possível alterar jornadas de meses anteriores.");

        if (journey.Inconsistent)
            return OperationResult.Failure("Esta jornada já está marcada como inconsistente.");

        var updated = new Journey
        {
            Id = journey.Id,
            PersonId = journey.PersonId,
            CheckIn = journey.CheckIn,
            CheckOut = journey.CheckOut,
            TotalMinutes = journey.TotalMinutes,
            Inconsistent = true,
            InconsistencyReason = model.Reason,
            CreatedAt = journey.CreatedAt
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await journeyRepository.UpdateAsync(updated, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OperationResult> DeleteAsync(Guid journeyId, CancellationToken cancellationToken = default)
    {
        var journey = await journeyRepository.GetByIdAsync(journeyId, cancellationToken);
        if (journey is null)
            return OperationResult.Failure("Jornada não encontrada.");

        if (!IsCurrentMonth(journey.CheckIn))
            return OperationResult.Failure("Não é possível excluir jornadas de meses anteriores.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await journeyRepository.DeleteAsync(journeyId, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static bool IsCurrentMonth(DateTimeOffset date)
    {
        var now = DateTimeOffset.UtcNow;
        return date.Year == now.Year && date.Month == now.Month;
    }

    private static JourneyResponseModel MapToResponse(Journey journey) => new()
    {
        Id = journey.Id,
        PersonId = journey.PersonId,
        CheckIn = journey.CheckIn,
        CheckOut = journey.CheckOut,
        TotalMinutes = journey.TotalMinutes,
        Inconsistent = journey.Inconsistent,
        InconsistencyReason = journey.InconsistencyReason,
        CreatedAt = journey.CreatedAt,
        Entries = journey.Entries.Select(e => new JourneyEntryResponseModel
        {
            Id = e.Id,
            CheckIn = e.CheckIn,
            CheckOut = e.CheckOut,
            Minutes = e.Minutes,
            BaseRate = e.BaseRate,
            GrossAmount = e.GrossAmount,
            Type = e.Type
        }).ToList()
    };
}
