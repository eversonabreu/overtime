using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed record OperationResult(bool IsSuccess, string? Reason = null)
{
    public static OperationResult Success() => new(true);
    public static OperationResult Failure(string reason) => new(false, reason);
}