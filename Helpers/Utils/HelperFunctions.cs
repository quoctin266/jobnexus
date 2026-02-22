using JobNexus.Common.Enum;

namespace JobNexus.Helpers.Utils
{
    public static class HelperFunctions
    {
        public static void SetRefreshTokenCookie(HttpResponse response, string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                IsEssential = true
            };

            response.Cookies.Append(TokenType.RefreshToken.ToString(), refreshToken, cookieOptions);
        }

        public static void DeleteRefreshTokenCookie(HttpResponse response)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true
            };

            response.Cookies.Delete(TokenType.RefreshToken.ToString(), cookieOptions);
        }
    }
}
