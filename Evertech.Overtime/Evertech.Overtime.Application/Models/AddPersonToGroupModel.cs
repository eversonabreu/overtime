using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class AddPersonToGroupModel
{
    public Guid GroupId { get; init; }
    public Guid PersonId { get; init; }
}
