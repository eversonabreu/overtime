using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class Country : Entity
{
    public string Name { get; init; } = string.Empty;
}
