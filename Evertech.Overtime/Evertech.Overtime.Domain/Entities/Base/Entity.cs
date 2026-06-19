using Evertech.Overtime.Domain.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities.Base;

[ExcludeFromCodeCoverage]
public class Entity
{
    [NotUpdatable]
    public Guid Id { get; init; }
}