using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CompensatoryConversionResponseModel
{
    public Guid Id { get; init; }
    public Guid PersonId { get; init; }
    public DateOnly ConversionDate { get; init; }
    public int Minutes { get; init; }
    public ConversionType Type { get; init; }
    public decimal RemuneratedAmount { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}
