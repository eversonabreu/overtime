using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Entities.Base;

[ExcludeFromCodeCoverage]
public class Entity
{
    public Guid Id { get; init; }
}