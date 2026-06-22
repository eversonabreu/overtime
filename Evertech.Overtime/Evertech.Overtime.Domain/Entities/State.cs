using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class State : Entity
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;

    [NotUpdatable]
    public Guid CountryId { get; init; }
}
