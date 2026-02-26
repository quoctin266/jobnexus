using JobNexus.Common.Enum;
using JobNexus.Models;
using System.Security.Claims;

namespace JobNexus.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(AppUser user);

        string CreateRefreshToken(Guid tokenIdentity, DateTime expiresAt);

        string CreateVerifyToken(Guid tokenIdentity, DateTime expiresAt, string email, TokenPurpose purpose);

        ClaimsPrincipal? ValidateToken(string token);
    }
}
