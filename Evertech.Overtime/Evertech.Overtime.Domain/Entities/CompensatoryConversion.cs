using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class CompensatoryConversion : Entity
{
    [NotUpdatable]
    public Guid PersonId { get; init; }

    public DateOnly ConversionDate { get; init; }
    public int Minutes { get; init; }
    public ConversionType Type { get; init; }

    [NotUpdatable]
    public DateTimeOffset CreatedAt { get; init; }
}