using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Domain.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class BusinessException(string message) : Exception(message);