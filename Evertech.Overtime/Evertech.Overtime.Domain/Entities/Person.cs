using Evertech.Overtime.Domain.Entities.Base;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Person : Entity
{
    public string Name { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
    public decimal HourlyRate { get; init; }
    public bool CompensatoryTimeEnabled { get; init; }
    public Guid MunicipalityId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}