using System.Diagnostics.CodeAnalysis;
using Evertech.Overtime.Domain.Attributes;
using Evertech.Overtime.Domain.Entities.Base;

namespace Evertech.Overtime.Domain.Entities;

[ExcludeFromCodeCoverage]
public sealed class PersonToken : Entity
{
    [NotUpdatable]
    public Guid PersonId { get; init; }

    [NotUpdatable]
    public string RefreshToken { get; init; } = string.Empty;

    [NotUpdatable]
    public DateTimeOffset ExpiresAt { get; init; }

    public bool IsRevoked { get; init; }

    [NotUpdatable]
    public DateTimeOffset CreatedAt { get; init; }
}