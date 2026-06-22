using Evertech.Overtime.Application.Models;
using Evertech.Overtime.Application.Services.Abstractions;
using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories;
using Evertech.Overtime.Domain.Services.Abstractions;

namespace Evertech.Overtime.Application.Services.Implementations;

internal sealed class HolidayAppService(
    INationalHolidayRepository nationalHolidayRepository,
    IStateHolidayRepository stateHolidayRepository,
    IMunicipalityHolidayRepository municipalityHolidayRepository,
    IDbUnitOfWork unitOfWork) : IHolidayAppService
{
    public async Task<Guid> CreateNationalAsync(CreateNationalHolidayModel model, CancellationToken cancellationToken = default)
    {
        var holiday = new NationalHoliday
        {
            Id = Guid.NewGuid(),
            Day = model.Day,
            Month = model.Month,
            Description = model.Description
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var id = await nationalHolidayRepository.AddAsync(holiday, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateNationalAsync(UpdateNationalHolidayModel model, CancellationToken cancellationToken = default)
    {
        var current = await nationalHolidayRepository.GetByIdAsync(model.Id, cancellationToken);
        if (current is null) return;

        var updated = new NationalHoliday
        {
            Id = current.Id,
            Day = model.Day,
            Month = model.Month,
            Description = model.Description
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await nationalHolidayRepository.UpdateAsync(updated, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteNationalAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await nationalHolidayRepository.DeleteAsync(id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<NationalHolidayResponseModel>> GetNationalByMonthAsync(int month, CancellationToken cancellationToken = default)
    {
        var holidays = await nationalHolidayRepository.GetByMonthAsync(month, cancellationToken);
        return holidays.Select(h => new NationalHolidayResponseModel
        {
            Id = h.Id,
            Day = h.Day,
            Month = h.Month,
            Description = h.Description
        }).ToList();
    }

    public async Task<Guid> CreateStateAsync(CreateStateHolidayModel model, CancellationToken cancellationToken = default)
    {
        var holiday = new StateHoliday
        {
            Id = Guid.NewGuid(),
            Day = model.Day,
            Month = model.Month,
            Description = model.Description,
            StateId = model.StateId
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var id = await stateHolidayRepository.AddAsync(holiday, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateStateAsync(UpdateStateHolidayModel model, CancellationToken cancellationToken = default)
    {
        var current = await stateHolidayRepository.GetByIdAsync(model.Id, cancellationToken);
        if (current is null) return;

        var updated = new StateHoliday
        {
            Id = current.Id,
            Day = model.Day,
            Month = model.Month,
            Description = model.Description,
            StateId = current.StateId
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await stateHolidayRepository.UpdateAsync(updated, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteStateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await stateHolidayRepository.DeleteAsync(id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<StateHolidayResponseModel>> GetStateByMonthAndMunicipalityAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        var holidays = await stateHolidayRepository.GetByMonthAndMunicipalityAsync(month, municipalityId, cancellationToken);
        return holidays.Select(h => new StateHolidayResponseModel
        {
            Id = h.Id,
            Day = h.Day,
            Month = h.Month,
            Description = h.Description,
            StateId = h.StateId
        }).ToList();
    }

    public async Task<Guid> CreateMunicipalityAsync(CreateMunicipalityHolidayModel model, CancellationToken cancellationToken = default)
    {
        var holiday = new MunicipalityHoliday
        {
            Id = Guid.NewGuid(),
            Day = model.Day,
            Month = model.Month,
            Description = model.Description,
            MunicipalityId = model.MunicipalityId
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var id = await municipalityHolidayRepository.AddAsync(holiday, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
            return id;
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateMunicipalityAsync(UpdateMunicipalityHolidayModel model, CancellationToken cancellationToken = default)
    {
        var current = await municipalityHolidayRepository.GetByIdAsync(model.Id, cancellationToken);
        if (current is null) return;

        var updated = new MunicipalityHoliday
        {
            Id = current.Id,
            Day = model.Day,
            Month = model.Month,
            Description = model.Description,
            MunicipalityId = current.MunicipalityId
        };

        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await municipalityHolidayRepository.UpdateAsync(updated, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task DeleteMunicipalityAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await municipalityHolidayRepository.DeleteAsync(id, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<MunicipalityHolidayResponseModel>> GetMunicipalityByMonthAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default)
    {
        var holidays = await municipalityHolidayRepository.GetByMonthAsync(month, municipalityId, cancellationToken);
        return holidays.Select(h => new MunicipalityHolidayResponseModel
        {
            Id = h.Id,
            Day = h.Day,
            Month = h.Month,
            Description = h.Description,
            MunicipalityId = h.MunicipalityId
        }).ToList();
    }
}
