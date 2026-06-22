using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class JourneyEntryResponseModel
{
    public Guid Id { get; init; }
    public DateTimeOffset CheckIn { get; init; }
    public DateTimeOffset CheckOut { get; init; }
    public int Minutes { get; init; }
    public short BaseRate { get; init; }
    public decimal GrossAmount { get; init; }
    public EntryType Type { get; init; }
}
