using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateGroupModel
{
    public string Name { get; init; } = string.Empty;
    public string? Observation { get; init; }
    public Guid LeaderPersonId { get; init; }
}
