using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class SetGroupLeaderModel
{
    public Guid GroupId { get; init; }
    public Guid PersonId { get; init; }
    public bool IsLeader { get; init; }
}
