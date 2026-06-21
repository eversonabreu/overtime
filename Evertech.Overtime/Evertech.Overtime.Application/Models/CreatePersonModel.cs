using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreatePersonModel
{
    public string Name { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public bool IsAdmin { get; init; }
    public decimal HourlyRate { get; init; }
    public bool CompensatoryTimeEnabled { get; init; }
    public Guid MunicipalityId { get; init; }
}
