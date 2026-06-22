using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Municipality : Entity
{
    public string Name { get; init; } = string.Empty;

    [NotUpdatable]
    public Guid StateId { get; init; }
}
