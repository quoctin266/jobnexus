using System.Security.Claims;

namespace JobNexus.Extensions
{
    public static class ClaimsExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("id");
        }

    }
}
