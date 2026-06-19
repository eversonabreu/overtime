using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Journey : Entity
{
    [NotUpdatable]
    public Guid PersonId { get; init; }

    [NotUpdatable]
    public DateTimeOffset CheckIn { get; init; }

    [NotUpdatable]
    public DateTimeOffset CheckOut { get; init; }

    [NotUpdatable]
    public int TotalMinutes { get; init; }

    public bool Inconsistent { get; init; }
    public string? InconsistencyReason { get; init; }

    [NotUpdatable]
    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<JourneyEntry> Entries { get; init; } = [];
}