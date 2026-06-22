using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Enums;
using Evertech.Overtime.Domain.Exceptions;
using Evertech.Overtime.Domain.Helpers;
using Evertech.Overtime.Domain.Interfaces;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Application.Services.Implementations;

internal sealed class CompensatoryConversionAppService(
    ICompensatoryConversionRepository conversionRepository,
    IPersonRepository personRepository,
    ICompensatoryConversionService conversionService,
    ICompensatoryBalanceService balanceService,
    IDbUnitOfWork unitOfWork) : ICompensatoryConversionAppService
{
    public async Task<Guid> CreateAsync(CreateCompensatoryConversionModel model, CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(model.PersonId, cancellationToken)
            ?? throw new BusinessException("Colaborador não encontrado.");

        if (!person.CompensatoryTimeEnabled)
            throw new BusinessException("Este colaborador não possui banco de horas habilitado.");

        var canConvert = await conversionService.CanConvertAsync(model.PersonId, model.Minutes, cancellationToken);
        if (!canConvert)
        {
            var balance = await balanceService.GetAvailableBalanceAsync(model.PersonId, cancellationToken);
            throw new BusinessException(
                $"Saldo insuficiente para conversão. Saldo disponível: {balance} minutos. Solicitado: {model.Minutes} minutos.");
        }

        var conversion = new CompensatoryConversion
        {
            Id = Guid.NewGuid(),
            PersonId = model.PersonId,
            ConversionDate = model.ConversionDate,
            Minutes = model.Minutes,
            Type = model.Type,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var id = await conversionRepository.AddAsync(conversion, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OperationResult> UpdateAsync(UpdateCompensatoryConversionModel model, CancellationToken cancellationToken = default)
    {
        var current = await conversionRepository.GetByIdAsync(model.Id, cancellationToken);
        if (current is null)
            return OperationResult.Failure("Conversão não encontrada.");

        if (!IsCurrentMonth(current.ConversionDate))
            return OperationResult.Failure("Não é possível alterar conversões de meses anteriores.");

        var canEdit = await conversionService.CanEditAsync(current.PersonId, current.Minutes, model.Minutes, cancellationToken);
        if (!canEdit)
        {
            var balance = await balanceService.GetAvailableBalanceAsync(current.PersonId, cancellationToken);
            return OperationResult.Failure(
                $"Saldo insuficiente para aumentar a conversão. Saldo disponível: {balance} minutos. " +
                $"Diferença solicitada: {model.Minutes - current.Minutes} minutos.");
        }

        var updated = new CompensatoryConversion
        {
            Id = current.Id,
            PersonId = current.PersonId,
            ConversionDate = model.ConversionDate,
            Minutes = model.Minutes,
            Type = model.Type,
            CreatedAt = current.CreatedAt
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await conversionRepository.UpdateAsync(updated, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var conversion = await conversionRepository.GetByIdAsync(id, cancellationToken);
        if (conversion is null)
            return OperationResult.Failure("Conversão não encontrada.");

        if (!IsCurrentMonth(conversion.ConversionDate))
            return OperationResult.Failure("Não é possível excluir conversões de meses anteriores.");

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await conversionRepository.DeleteAsync(id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<CompensatoryConversionResponseModel>> GetByPersonAndMonthAsync(
        Guid personId,
        int year,
        int month,
        CancellationToken cancellationToken = default)
    {
        var person = await personRepository.GetByIdAsync(personId, cancellationToken);
        var hourlyRate = person?.HourlyRate ?? 0m;

        var conversions = await conversionRepository.GetByPersonAndMonthAsync(personId, year, month, cancellationToken);

        return conversions.Select(c => MapToResponse(c, hourlyRate)).ToList();
    }

    public async Task<CompensatoryBalanceResponseModel> GetBalanceAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        var accumulated = await conversionRepository.GetAccumulatedMinutesAsync(personId, cancellationToken);
        var converted = await conversionRepository.GetConvertedMinutesAsync(personId, cancellationToken);
        var available = accumulated - converted;

        return new CompensatoryBalanceResponseModel
        {
            PersonId = personId,
            AccumulatedMinutes = accumulated,
            ConvertedMinutes = converted,
            AvailableMinutes = available,
            AvailableHours = RoundingHelper.RoundHalfUp(available / 60m)
        };
    }

    private static bool IsCurrentMonth(DateOnly date)
    {
        var now = DateOnly.FromDateTime(DateTime.UtcNow);
        return date.Year == now.Year && date.Month == now.Month;
    }

    private static CompensatoryConversionResponseModel MapToResponse(CompensatoryConversion conversion, decimal hourlyRate) => new()
    {
        Id = conversion.Id,
        PersonId = conversion.PersonId,
        ConversionDate = conversion.ConversionDate,
        Minutes = conversion.Minutes,
        Type = conversion.Type,
        RemuneratedAmount = conversion.Type == ConversionType.Remunerated
            ? RoundingHelper.RoundMonetary((hourlyRate / 60m) * 1.50m * conversion.Minutes)
            : 0m,
        CreatedAt = conversion.CreatedAt
    };
}
