using Evertech.Overtime.Application.Models;

namespace Evertech.Overtime.Application.Services.Abstractions;

public interface IHolidayAppService
{
    Task<Guid> CreateNationalAsync(CreateNationalHolidayModel model, CancellationToken cancellationToken = default);
    Task UpdateNationalAsync(UpdateNationalHolidayModel model, CancellationToken cancellationToken = default);
    Task DeleteNationalAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<NationalHolidayResponseModel>> GetNationalByMonthAsync(int month, CancellationToken cancellationToken = default);

    Task<Guid> CreateStateAsync(CreateStateHolidayModel model, CancellationToken cancellationToken = default);
    Task UpdateStateAsync(UpdateStateHolidayModel model, CancellationToken cancellationToken = default);
    Task DeleteStateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<StateHolidayResponseModel>> GetStateByMonthAndMunicipalityAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default);

    Task<Guid> CreateMunicipalityAsync(CreateMunicipalityHolidayModel model, CancellationToken cancellationToken = default);
    Task UpdateMunicipalityAsync(UpdateMunicipalityHolidayModel model, CancellationToken cancellationToken = default);
    Task DeleteMunicipalityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<MunicipalityHolidayResponseModel>> GetMunicipalityByMonthAsync(int month, Guid municipalityId, CancellationToken cancellationToken = default);
}
