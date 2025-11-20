using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using ToDoTaskManagement.Data;
using ToDoTaskManagement.Domain.Entities;
using ToDoTaskManagement.Domain.Interfaces;

namespace ToDoTaskManagement.Infrastructure.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly TodoDbContext _db;
        private readonly ILogger<RefreshTokenService> _logger;
        public RefreshTokenService(TodoDbContext db, ILogger<RefreshTokenService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<RefreshToken> CreateAsync(string userId, string ipAddress, int daysToExpire = 30)
        {
            var rt = new RefreshToken
            {
                Token = Guid.NewGuid().ToString("N") + Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(daysToExpire),
                RemoteIpAddress = ipAddress
            };
            await _db.RefreshTokens.AddAsync(rt);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Created refresh token for user {UserId} tokenId={TokenId}", userId, rt.Id);
            return rt;
        }

        public Task<RefreshToken?> GetByTokenAsync(string token) =>
            _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token && !x.Revoked);

        public async Task RevokeAsync(RefreshToken token, string replacedBy = null)
        {
            token.Revoked = true;
            token.ReplacedByToken = replacedBy;
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync();
            _logger.LogInformation("Revoked refresh token id={Id} replacedBy={Replaced}", token.Id, replacedBy);
        }
    }
}
