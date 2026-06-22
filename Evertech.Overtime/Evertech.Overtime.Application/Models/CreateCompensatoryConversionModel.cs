using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Enums;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateCompensatoryConversionModel
{
    public Guid PersonId { get; init; }
    public DateOnly ConversionDate { get; init; }
    public int Minutes { get; init; }
    public ConversionType Type { get; init; }
}
