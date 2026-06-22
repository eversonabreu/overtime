using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateStateHolidayModel
{
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid StateId { get; init; }
}

[ExcludeFromCodeCoverage]
public sealed class UpdateStateHolidayModel
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class StateHolidayResponseModel
{
    public Guid Id { get; init; }
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
    public Guid StateId { get; init; }
}
