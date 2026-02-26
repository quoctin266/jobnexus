using System.Security.Claims;

namespace JobNexus.Extensions
{
    public static class ClaimsExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user.FindFirstValue("id");
        }

        public static string? GetTokenIdentity(this ClaimsPrincipal token)
        {
            return token.FindFirstValue("tokenIdentity");
        }

        public static string? GetTokenEmail(this ClaimsPrincipal token)
        {
            return token.FindFirstValue(ClaimTypes.Email);
        }

        public static string? GetTokenPurpose(this ClaimsPrincipal token)
        {
            return token.FindFirstValue("purpose");
        }

    }
}
