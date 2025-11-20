using System.Security.Claims;

namespace ToDoTaskManagement.Api.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string userId, IEnumerable<Claim>? additionalClaims = null);
    }
}
