using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class JourneyResponseModel
{
    public Guid Id { get; init; }
    public Guid PersonId { get; init; }
    public DateTimeOffset CheckIn { get; init; }
    public DateTimeOffset CheckOut { get; init; }
    public int TotalMinutes { get; init; }
    public bool Inconsistent { get; init; }
    public string? InconsistencyReason { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public IReadOnlyList<JourneyEntryResponseModel> Entries { get; init; } = [];
}
