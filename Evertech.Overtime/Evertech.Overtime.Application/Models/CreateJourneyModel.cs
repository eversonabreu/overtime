using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateJourneyModel
{
    public Guid PersonId { get; init; }
    public DateTimeOffset CheckIn { get; init; }
    public DateTimeOffset CheckOut { get; init; }
}
