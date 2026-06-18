using Evertech.Overtime.Domain.Entities.Base;
using Evertech.Overtime.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Holiday : Entity
{
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
    public HolidayOrigin Origin { get; init; }
    public Guid ReferenceId { get; init; }
}