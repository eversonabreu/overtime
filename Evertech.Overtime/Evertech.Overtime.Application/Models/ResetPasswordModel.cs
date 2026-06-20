using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class ResetPasswordModel
{
    public Guid PersonId { get; init; }
    public string NewPassword { get; init; } = string.Empty;
}