using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class GroupPerson : Entity
{
    [NotUpdatable]
    public Guid GroupId { get; init; }

    [NotUpdatable]
    public Guid PersonId { get; init; }

    public bool IsLeader { get; init; }
}