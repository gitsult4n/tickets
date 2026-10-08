using System.Security.Claims;

namespace tickets.Api.Common;

public static class ClaimsExtension
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public static string GetUserName(this ClaimsPrincipal user) =>
        user.Identity!.Name!;
}
