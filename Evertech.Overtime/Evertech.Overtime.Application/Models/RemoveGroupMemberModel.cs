using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class RemoveGroupMemberModel
{
    public Guid GroupId { get; init; }
    public Guid PersonId { get; init; }
}
