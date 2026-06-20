using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class RequestPasswordResetModel
{
    public string Email { get; init; } = string.Empty;
}