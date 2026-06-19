using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class NationalHoliday : Entity
{
    public int Day { get; init; }
    public int Month { get; init; }
    public string Description { get; init; } = string.Empty;
}