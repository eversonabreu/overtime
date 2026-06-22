using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class MarkJourneyInconsistentModel
{
    public Guid JourneyId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
