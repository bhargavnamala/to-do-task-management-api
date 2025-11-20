
using ToDoTaskManagement.Domain.Entities;

namespace ToDoTaskManagement.Domain.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> CreateAsync(string userId, string ipAddress, int daysToExpire = 30);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task RevokeAsync(RefreshToken token, string replacedBy = null);
    }
}
