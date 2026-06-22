using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateMunicipalityHolidayModel
{
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid MunicipalityId { get; init; }
}

[ExcludeFromCodeCoverage]
public sealed class UpdateMunicipalityHolidayModel
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class MunicipalityHolidayResponseModel
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid MunicipalityId { get; init; }
}
