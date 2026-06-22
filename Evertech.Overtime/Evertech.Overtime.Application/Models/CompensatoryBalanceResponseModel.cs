using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CompensatoryBalanceResponseModel
{
    public Guid PersonId { get; init; }
    public int AccumulatedMinutes { get; init; }
    public int ConvertedMinutes { get; init; }
    public int AvailableMinutes { get; init; }
    public decimal AvailableHours { get; init; }
}
