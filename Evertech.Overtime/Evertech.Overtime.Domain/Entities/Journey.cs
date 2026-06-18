using Evertech.Overtime.Domain.Entities.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Journey : Entity
{
    public Guid PersonId { get; init; }
    public DateTimeOffset CheckIn { get; init; }
    public DateTimeOffset CheckOut { get; init; }
    public int TotalMinutes { get; init; }
    public bool Inconsistent { get; init; }
    public string? InconsistencyReason { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<JourneyEntry> Entries { get; init; } = [];
}