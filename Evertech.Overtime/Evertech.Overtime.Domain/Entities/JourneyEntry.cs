using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class JourneyEntry : Entity
{
    [NotUpdatable]
    public Guid JourneyId { get; init; }

    [NotUpdatable]
    public DateTimeOffset CheckIn { get; init; }

    [NotUpdatable]
    public DateTimeOffset CheckOut { get; init; }

    [NotUpdatable]
    public int Minutes { get; init; }

    [NotUpdatable]
    public short BaseRate { get; init; }

    [NotUpdatable]
    public decimal GrossAmount { get; init; }

    [NotUpdatable]
    public EntryType Type { get; init; }

    [NotUpdatable]
    public DateTimeOffset CreatedAt { get; init; }
}