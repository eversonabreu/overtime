using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace Evertech.Overtime.API.Extensions;

[ExcludeFromCodeCoverage]
public static class ClaimsPrincipalExtensions
{
    public static Guid GetPersonId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub")
            ?? throw new InvalidOperationException("PersonId não encontrado no token.");

        return Guid.Parse(value);
    }

    public static bool GetIsAdmin(this ClaimsPrincipal user) =>
        string.Equals(user.FindFirstValue("isAdmin"), "true", StringComparison.OrdinalIgnoreCase);

    public static bool GetIsLeader(this ClaimsPrincipal user) =>
        string.Equals(user.FindFirstValue("isLeader"), "true", StringComparison.OrdinalIgnoreCase);

    public static IReadOnlyList<Guid> GetLeaderOfGroups(this ClaimsPrincipal user) =>
        user.FindAll("leaderOfGroups")
            .Select(c => Guid.Parse(c.Value))
            .ToList();

    public static bool IsLeaderOfGroup(this ClaimsPrincipal user, Guid groupId) =>
        user.GetLeaderOfGroups().Contains(groupId);
}
