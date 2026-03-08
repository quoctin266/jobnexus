using System.Security.Claims;

namespace JobNexus.Extensions
{
    public static class ClaimsExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue("id");
        }

        public static string? GetTokenIdentity(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue("tokenIdentity");
        }

        public static string? GetEmail(this ClaimsPrincipal claims)
        {
            return claims.FindFirstValue(ClaimTypes.Email);
        }
    }
}
