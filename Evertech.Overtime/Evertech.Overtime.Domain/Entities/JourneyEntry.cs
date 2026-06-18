using Evertech.Overtime.Domain.Entities.Base;
using Evertech.Overtime.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class JourneyEntry : Entity
{
    public Guid JourneyId { get; init; }
    public DateTimeOffset CheckIn { get; init; }
    public DateTimeOffset CheckOut { get; init; }
    public int Minutes { get; init; }
    public short BaseRate { get; init; }
    public decimal GrossAmount { get; init; }
    public EntryType Type { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}