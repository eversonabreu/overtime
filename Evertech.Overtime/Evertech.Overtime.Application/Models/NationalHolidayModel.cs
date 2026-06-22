using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateNationalHolidayModel
{
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class UpdateNationalHolidayModel
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class NationalHolidayResponseModel
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
}
