using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(AppUser user);

        string CreateRefreshToken(Guid tokenIdentity, DateTime expiresAt);

        ClaimsPrincipal? ValidateToken(string token);
    }
}
