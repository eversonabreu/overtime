using Evertech.Overtime.Domain.Entities;
using Evertech.Overtime.Domain.Repositories.Base;

namespace Evertech.Overtime.Domain.Repositories;

public interface ICountryRepository : IRepository<Country>
{
    Task<IReadOnlyList<Country>> GetAllOrderedAsync(CancellationToken cancellationToken = default);
}

public interface IStateRepository : IRepository<State>
{
    Task<IReadOnlyList<State>> GetByCountryIdAsync(Guid countryId, CancellationToken cancellationToken = default);
}

public interface IMunicipalityRepository : IRepository<Municipality>
{
    Task<IReadOnlyList<Municipality>> GetByStateIdAsync(Guid stateId, CancellationToken cancellationToken = default);
}
