using JobNexus.Data;
using JobNexus.Interfaces.Repository;
using JobNexus.Models;
using Microsoft.EntityFrameworkCore;

namespace JobNexus.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly ApplicationDBContext _context;

        public TokenRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<Token> CreateAsync(Token token)
        {
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();

            return token;
        }

        public async Task DeleteAsync(Token token)
        {
            _context.Tokens.Remove(token);

            await _context.SaveChangesAsync();
        }

        public async Task<Token?> GetByIdentityAsync(Guid identity)
        {
            return await _context.Tokens.FirstOrDefaultAsync(t => t.TokenIdentity == identity);
        }

        public async Task<Token> UpdateAsync(Token token, Guid newIdentity, DateTimeOffset newExpiresAt)
        {
            token.TokenIdentity = newIdentity;
            token.ExpiresAt = newExpiresAt;

            await _context.SaveChangesAsync();

            return token;
        }
    }
}
