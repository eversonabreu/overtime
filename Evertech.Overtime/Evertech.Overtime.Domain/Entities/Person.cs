using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Person : Entity
{
    public string Name { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    [NotUpdatable]
    public string Password { get; init; } = string.Empty;

    public bool IsActive { get; init; }
    public bool IsPasswordPendingReset { get; init; }
    public bool IsBlackTheme { get; init; }

    [NotUpdatable]
    public bool IsAdmin { get; init; }

    public decimal HourlyRate { get; init; }
    public bool CompensatoryTimeEnabled { get; init; }
    public Guid MunicipalityId { get; init; }

    [NotUpdatable]
    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset UpdatedAt { get; init; }
}
