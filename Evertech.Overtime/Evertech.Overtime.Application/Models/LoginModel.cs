using System.Diagnostics.CodeAnalysis;

namespace Evertech.Overtime.Application.Models;

[ExcludeFromCodeCoverage]
public sealed class LoginModel
{
    public string Credential { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class RefreshTokenModel
{
    public string RefreshToken { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class RevokeTokenModel
{
    public string RefreshToken { get; init; } = string.Empty;
}

[ExcludeFromCodeCoverage]
public sealed class TokenResponseModel
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTimeOffset AccessTokenExpiresAt { get; init; }
}