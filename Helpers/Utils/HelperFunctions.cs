using JobNexus.Common.Enum;
using System.Reflection;

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

        public static async Task<string> ReadTemplateAsync(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // resource name format: {DefaultNamespace}.Templates.FileName
            var resourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(x => x.EndsWith($"Templates.{templateName}"));

            if (resourceName == null)
                throw new FileNotFoundException("Template not found.", templateName);

            await using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var reader = new StreamReader(stream);

            return await reader.ReadToEndAsync();
        }
    }
}
