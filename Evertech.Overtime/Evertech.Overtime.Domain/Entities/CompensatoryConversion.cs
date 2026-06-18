using Evertech.Overtime.Domain.Entities.Base;
using Evertech.Overtime.Domain.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class CompensatoryConversion : Entity
{
    public Guid PersonId { get; init; }
    public DateOnly ConversionDate { get; init; }
    public int Minutes { get; init; }
    public ConversionType Type { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}