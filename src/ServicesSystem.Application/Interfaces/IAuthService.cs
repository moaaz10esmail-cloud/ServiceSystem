using ServicesSystem.Domain.Entities;
using ServicesSystem.Shared.Common;
using ServicesSystem.Shared.DTOs.Auth;

namespace ServicesSystem.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
    Task<Result> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
}
