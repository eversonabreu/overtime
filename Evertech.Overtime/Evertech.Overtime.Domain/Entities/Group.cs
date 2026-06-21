using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Group : Entity
{
    public string Name { get; init; } = string.Empty;
    public string Observation { get; init; }
    public bool IsActive { get; init; }
}