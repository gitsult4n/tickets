using System.Security.Claims;
using static System.Security.Claims.ClaimTypes;

namespace tickets.Api.Common;

public static class ClaimsExtension
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(NameIdentifier)!);

    public static string GetUserName(this ClaimsPrincipal user) => user.Identity!.Name!;

    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
}
