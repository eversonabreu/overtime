using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class PersonResponseModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsActive { get; init; }
    public bool IsPasswordPendingReset { get; init; }
    public decimal HourlyRate { get; init; }
    public bool CompensatoryTimeEnabled { get; init; }
    public Guid MunicipalityId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}