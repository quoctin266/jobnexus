using JobNexus.Common.Enum;
using JobNexus.Models;

namespace JobNexus.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user, TokenType tokenType);
    }
}
