using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class GroupResponseModel
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Observation { get; init; }
    public bool IsActive { get; init; }
}
