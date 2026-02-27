using JobNexus.Common.Enum;
using JobNexus.Models;

namespace JobNexus.Interfaces.Repository
{
    public interface ITokenRepository
    {
        Task<Token?> GetByIdentityAsync(Guid identity);

        Task<Token?> GetByUserAndPurposeAsync(string userId, TokenPurpose purpose);

        Task<Token> CreateAsync(Token token);

        Task<Token> UpdateAsync(Token token, Guid newIdentity, DateTimeOffset newExpiresAt);

        Task DeleteAsync(Token token);
    }
}
