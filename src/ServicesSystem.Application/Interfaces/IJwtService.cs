using ServicesSystem.Domain.Entities;

namespace ServicesSystem.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? GetUserIdFromToken(string token);
    bool ValidateToken(string token);
}
