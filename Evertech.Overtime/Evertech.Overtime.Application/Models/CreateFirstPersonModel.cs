using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class CreateFirstPersonModel
{
    public string Name { get; init; } = string.Empty;
    public string Registration { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}